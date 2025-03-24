namespace BusMonitor.DTOs
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int TripId { get; set; }
    }
} 