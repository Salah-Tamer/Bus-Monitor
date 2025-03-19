using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusMonitor.Models
{
    public class Trip
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public Status Status { get; set; } // Planned, Active, Completed

        [ForeignKey("Bus")]
        public int BusId { get; set; }

        [ForeignKey("Route")]
        public int RouteId { get; set; }

        [ForeignKey("Driver")]
        public int? DriverId { get; set; }

        [ForeignKey("Supervisor")]
        public int? SupervisorId { get; set; }

        [ForeignKey("Admin")]
        public int? AdminId { get; set; }

        // Navigation properties
        public virtual Bus Bus { get; set; }
        public virtual Route Route { get; set; }
        public virtual User Driver { get; set; }
        public virtual User Supervisor { get; set; }
        public virtual User Admin { get; set; }
        public virtual ICollection<StudentTrip> StudentTrips { get; set; }

        public Trip()
        {
            StudentTrips = new HashSet<StudentTrip>();
        }
    }
}
