using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.API.Controllers
{

    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new 
                { 
                    success = false, 
                    message = "Dữ liệu không hợp lệ", 
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                });
            }

            try
            {
                var result = await _feedbackService.CreateFeedbackAsync(request);
                return Ok(new { success = true, message = "Gửi đánh giá thành công", data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Gửi đánh giá thất bại", error = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllFeedbacks(
            [FromQuery] int? minScore = null,
            [FromQuery] int? maxScore = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _feedbackService.GetAllFeedbacksAsync();
                
                // Apply filters
                var filteredResult = result.AsQueryable();
                
                if (minScore.HasValue)
                    filteredResult = filteredResult.Where(f => f.Score >= minScore.Value);
                
                if (maxScore.HasValue)
                    filteredResult = filteredResult.Where(f => f.Score <= maxScore.Value);
                
                if (fromDate.HasValue)
                    filteredResult = filteredResult.Where(f => f.CreatedAt >= fromDate.Value);
                
                if (toDate.HasValue)
                    filteredResult = filteredResult.Where(f => f.CreatedAt <= toDate.Value);

                var pagedResult = filteredResult
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new 
                { 
                    success = true,
                    totalRecords = filteredResult.Count(),
                    averageScore = filteredResult.Any() ? filteredResult.Average(f => f.Score) : 0,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(filteredResult.Count() / (double)pageSize),
                    filters = new { minScore, maxScore, fromDate, toDate },
                    data = pagedResult 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Không thể lấy danh sách đánh giá", error = ex.Message });
            }
        }

        [HttpGet("{feedbackId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeedbackById(string feedbackId)
        {
            if (!Guid.TryParse(feedbackId, out var guid))
            {
                return Ok(new { success = false, message = "Mã đánh giá không hợp lệ" });
            }

            var result = await _feedbackService.GetFeedbackByIdAsync(guid);
            
            if (result == null)
            {
                return Ok(new { success = false, message = "Không tìm thấy đánh giá" });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpGet("rental/{rentalId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeedbacksByRentalId(string rentalId)
        {
            if (!Guid.TryParse(rentalId, out var guid))
            {
                return Ok(new { success = false, message = "Mã đơn thuê không hợp lệ" });
            }

            try
            {
                var result = await _feedbackService.GetFeedbacksByRentalIdAsync(guid);
                return Ok(new 
                { 
                    success = true,
                    rentalId = rentalId,
                    totalFeedbacks = result.Count,
                    averageScore = result.Any() ? result.Average(f => f.Score) : 0,
                    data = result 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Không thể lấy danh sách đánh giá", error = ex.Message });
            }
        }

        [HttpGet("renter/{renterId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeedbacksByRenterId(
            string renterId,
            [FromQuery] int? minScore = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (!Guid.TryParse(renterId, out var guid))
            {
                return Ok(new { success = false, message = "Mã người thuê không hợp lệ" });
            }

            try
            {
                var result = await _feedbackService.GetFeedbacksByRenterIdAsync(guid);
                
                if (minScore.HasValue)
                    result = result.Where(f => f.Score >= minScore.Value).ToList();

                var pagedResult = result
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new 
                { 
                    success = true,
                    renterId = renterId,
                    totalFeedbacks = result.Count,
                    averageScore = result.Any() ? result.Average(f => f.Score) : 0,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(result.Count / (double)pageSize),
                    filters = new { minScore },
                    data = pagedResult 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Không thể lấy lịch sử đánh giá", error = ex.Message });
            }
        }
    }
}

