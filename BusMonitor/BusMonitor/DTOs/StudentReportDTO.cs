namespace BusMonitor.DTOs
{
    public class StudentReportDTO
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int TripId { get; set; }
        public int SupervisorId { get; set; }
        public DateTime ReportDate { get; set; }
        public bool IsPresent { get; set; }
        public string BehaviorCategory { get; set; }
        public string BehaviorNotes { get; set; }
    }

    public class CreateStudentReportDTO
    {
        public int StudentId { get; set; }
        public bool IsPresent { get; set; }
        public string BehaviorCategory { get; set; }
        public string BehaviorNotes { get; set; }
    }
} 