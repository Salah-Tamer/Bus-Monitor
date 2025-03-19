using System.ComponentModel.DataAnnotations;

namespace BusMonitor.Models
{
    public class Bus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Number { get; set; }

        public int Capacity { get; set; }

        // Navigation properties
        public virtual ICollection<Trip> Trips { get; set; }

        public Bus()
        {
            Trips = new HashSet<Trip>();
        }
    }
}
