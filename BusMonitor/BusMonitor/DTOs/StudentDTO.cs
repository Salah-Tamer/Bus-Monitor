using System.ComponentModel.DataAnnotations;

namespace BusMonitor.DTOs
{
    public class StudentDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public string ParentPhoneNumber { get; set; }
    }

    // DTO for students with route information
    public class StudentWithRouteDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
        public string ParentPhoneNumber { get; set; }
        public string RouteStops { get; set; }
    }
} 