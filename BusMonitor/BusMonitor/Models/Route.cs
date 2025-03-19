using System.ComponentModel.DataAnnotations;

namespace BusMonitor.Models
{
    public class Route
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string StartPoint { get; set; }

        [Required]
        [StringLength(100)]
        public string EndPoint { get; set; }

        [StringLength(500)]
        public string Stops { get; set; }

        // Navigation properties
        public virtual ICollection<Trip> Trips { get; set; }

        public Route()
        {
            Trips = new HashSet<Trip>();
        }
    }
}
