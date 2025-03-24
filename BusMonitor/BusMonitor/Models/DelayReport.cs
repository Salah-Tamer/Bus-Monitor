using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusMonitor.Models
{
    public class DelayReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Trip")]
        public int TripId { get; set; }

        [Required]
        [ForeignKey("Supervisor")]
        public int SupervisorId { get; set; }

        [Required]
        public DateTime ReportDate { get; set; }

        [Required]
        [StringLength(200)]
        public string DelayReason { get; set; }

        [StringLength(500)]
        public string AdditionalNotes { get; set; }

        // Navigation properties
        public virtual Trip Trip { get; set; }
        public virtual User Supervisor { get; set; }
    }
} 