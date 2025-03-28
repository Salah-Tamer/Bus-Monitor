namespace BusMonitor.DTOs
{
    public class TripDTO
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int BusId { get; set; }
        public string BusNumber { get; set; }
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public int? DriverId { get; set; }
        public string DriverName { get; set; }
        public int? SupervisorId { get; set; }
        public string SupervisorName { get; set; }
        public int? AdminId { get; set; }
        public string AdminName { get; set; }
        public ICollection<StudentDTO> Students { get; set; }

        // New properties for tracking arrival and departure times
        public DateTime? ArrivalTime { get; set; }
        public DateTime? DepartureTime { get; set; }
    }

    public class CreateTripDTO
    {
        public int BusId { get; set; }
        public int RouteId { get; set; }
        public int? DriverId { get; set; }
        public int? SupervisorId { get; set; }
        public ICollection<int> StudentIds { get; set; }
    }

    public class UpdateTripDTO
    {
        public int BusId { get; set; }
        public int RouteId { get; set; }
        public int? DriverId { get; set; }
        public int? SupervisorId { get; set; }
        public string Status { get; set; }
    }
} 