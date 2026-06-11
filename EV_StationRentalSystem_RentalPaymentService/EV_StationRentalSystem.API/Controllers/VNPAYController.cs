using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.Helpers;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EV_StationRentalSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VNPAYController : ControllerBase
    {
        private readonly VNPAY _vnpay;
        private readonly Utils _utils;
        private readonly IPaymentService _paymentService;
        private readonly IRentalOrderService _rentalOrderService;

        public VNPAYController(
            IOptionsMonitor<VNPAY> vnpay, 
            Utils utils, 
            IPaymentService paymentService,
            IRentalOrderService rentalOrderService)
        {
            _vnpay = vnpay.CurrentValue;
            _utils = utils;
            _paymentService = paymentService;
            _rentalOrderService = rentalOrderService;
        }

        /// <summary>
        /// Tạo URL thanh toán VNPAY cho đơn thuê
        /// </summary>
        [HttpPost("payment-createURL")]
        public async Task<IActionResult> CreateVNPAYURL([FromBody] CreateVNPAYPaymentRequest request)
        {
            try
            {
                string vnp_ReturnUrl = _vnpay.VnPayReturnUrl; 
                string vnp_Url = _vnpay.VnPayUrl; 
                string vnp_TmnCode = _vnpay.VnPayTmnCode; 
                string vnp_HashSecret = _vnpay.VnPayHashSecret; 
                
                if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
                {
                    return Ok(new { success = false, message = "Vui lòng cấu hình các tham số VNPAY" });
                }

                // Tạo payment code duy nhất
                var paymentCode = Guid.NewGuid().ToString();
                
                // Khởi tạo VNPAYLibrary
                var vnpay = new VNPAYLibrary();
                vnpay.AddRequestData("vnp_Version", "2.1.0");
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", (request.Amount * 100).ToString()); // VNPAY yêu cầu nhân 100
                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", _utils.GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don thue xe {request.RentalId}");
                vnpay.AddRequestData("vnp_OrderType", "billpayment");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
                vnpay.AddRequestData("vnp_TxnRef", paymentCode); // Mã giao dịch

                // Tạo URL thanh toán
                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                
                return Ok(new 
                { 
                    success = true, 
                    message = "Tạo URL thanh toán thành công",
                    data = new 
                    {
                        paymentCode = paymentCode,
                        paymentUrl = paymentUrl 
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Tạo URL thanh toán thất bại", error = ex.Message });
            }
        }

        /// <summary>
        /// IPN - Instant Payment Notification từ VNPAY
        /// </summary>
        [HttpPost("payment-IPN")]
        public async Task<IActionResult> VnPayIpnUrl([FromQuery] Dictionary<string, string> queryParams)
        {
            try
            {
                string vnp_HashSecret = _vnpay.VnPayHashSecret;
                VNPAYLibrary vnpay = new VNPAYLibrary();

                // Lấy tất cả tham số có prefix "vnp_"
                foreach (var param in queryParams)
                {
                    if (!string.IsNullOrEmpty(param.Key) && param.Key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(param.Key, param.Value);
                    }
                }

                // Lấy thông tin từ query string
                string paymentCode = vnpay.GetResponseData("vnp_TxnRef");
                string vnpayTranId = vnpay.GetResponseData("vnp_TransactionNo");
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                string vnp_SecureHash = queryParams.ContainsKey("vnp_SecureHash") ? queryParams["vnp_SecureHash"] : null;
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;

                // Xác thực chữ ký
                bool isSignatureValid = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                if (!isSignatureValid)
                {
                    return Ok(new { RspCode = "97", Message = "Invalid signature" });
                }

                // Kiểm tra payment có tồn tại không
                var payment = await _paymentService.GetPaymentByTransactionCodeAsync(paymentCode);
                
                if (payment == null)
                {
                    return Ok(new { RspCode = "01", Message = "Order not found" });
                }

                if (payment.Amount != vnp_Amount)
                {
                    return Ok(new { RspCode = "04", Message = "Invalid amount" });
                }

                // Cập nhật trạng thái đơn hàng
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    // Thanh toán thành công
                    return Ok(new { RspCode = "00", Message = "Confirm Success" });
                }
                else
                {
                    // Thanh toán thất bại
                    return Ok(new { RspCode = "02", Message = "Payment failed" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { RspCode = "99", Message = ex.Message });
            }
        }

        /// <summary>
        /// Return URL - URL người dùng được redirect về sau khi thanh toán
        /// </summary>
        [HttpGet("payment-returnURL")]
        public async Task<IActionResult> ReturnURL([FromQuery] Dictionary<string, string> queryParams)
        {
            try
            {
                string vnp_HashSecret = _vnpay.VnPayHashSecret;
                VNPAYLibrary vnpay = new VNPAYLibrary();

                // Lấy tất cả tham số có prefix "vnp_"
                foreach (var param in queryParams)
                {
                    if (!string.IsNullOrEmpty(param.Key) && param.Key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(param.Key, param.Value);
                    }
                }

                // Lấy thông tin từ query string
                string paymentCode = vnpay.GetResponseData("vnp_TxnRef");
                string vnpayTranId = vnpay.GetResponseData("vnp_TransactionNo");
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                string vnp_SecureHash = queryParams.ContainsKey("vnp_SecureHash") ? queryParams["vnp_SecureHash"] : null;
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;

                // Kiểm tra chữ ký
                bool isSignatureValid = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                if (!isSignatureValid)
                {
                    return Redirect(_vnpay.URLFail + "?message=Invalid signature");
                }

                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    // Thanh toán thành công
                    return Redirect(_vnpay.URLSuccess + $"?paymentCode={paymentCode}&amount={vnp_Amount}");
                }

                // Thanh toán thất bại
                return Redirect(_vnpay.URLFail + $"?paymentCode={paymentCode}&responseCode={vnp_ResponseCode}");
            }
            catch (Exception ex)
            {
                return Redirect(_vnpay.URLFail + $"?message={ex.Message}");
            }
        }
    }

    // DTO cho request tạo payment URL
    public class CreateVNPAYPaymentRequest
    {
        public Guid RentalId { get; set; }
        public decimal Amount { get; set; }
    }
}

