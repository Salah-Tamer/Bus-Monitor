using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusMonitor.Models
{
    public class StudentReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Trip")]
        public int TripId { get; set; }

        [Required]
        [ForeignKey("Supervisor")]
        public int SupervisorId { get; set; }

        [Required]
        public DateTime ReportDate { get; set; }

        [Required]
        public bool IsPresent { get; set; }

        [Required]
        public BehaviorCategory BehaviorCategory { get; set; }

        [StringLength(500)]
        public string BehaviorNotes { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual User Supervisor { get; set; }
    }
} 