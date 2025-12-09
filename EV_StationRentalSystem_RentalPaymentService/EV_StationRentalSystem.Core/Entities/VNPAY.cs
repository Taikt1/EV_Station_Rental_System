namespace EV_StationRentalSystem.Core.Entities
{
    public class VNPAY
    {
        //Class ánh xạ IOptionMonitor
        public string VnPayUrl { get; set; }
        public string VnPayTmnCode { get; set; }
        public string VnPayHashSecret { get; set; }
        public string VnPayReturnUrl { get; set; }

        public string URLSuccess { get; set; }
        public string URLFail { get; set; }
    }
}

