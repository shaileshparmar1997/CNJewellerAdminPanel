namespace CNJewellerAdmin.DTOs.Base
{
    public class DaysCountResponse
    {
        public int WorkingDayCount { get; set; }
        public int TotalDayCount { get; set; }
        public List<DateTime> WorkingDays { get; set; }
        public List<DateTime> TotalDays { get; set; }
    }
}
