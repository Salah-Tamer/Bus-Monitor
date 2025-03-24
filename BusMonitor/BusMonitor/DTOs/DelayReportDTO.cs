namespace BusMonitor.DTOs
{
    public class DelayReportDTO
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int SupervisorId { get; set; }
        public DateTime ReportDate { get; set; }
        public string DelayReason { get; set; }
        public string AdditionalNotes { get; set; }
    }

    public class CreateDelayReportDTO
    {
        public int TripId { get; set; }
        public string DelayReason { get; set; }
        public string AdditionalNotes { get; set; }
    }
} 