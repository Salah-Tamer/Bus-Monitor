using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusMonitor.Models
{
    public class StudentTrip
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [ForeignKey("Trip")]
        public int TripId { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Trip Trip { get; set; }
    }
}
