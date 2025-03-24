using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusMonitor.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Parent")]
        public int ParentId { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [ForeignKey("Trip")]
        public int TripId { get; set; }

        // Navigation properties
        public virtual User Parent { get; set; }
        public virtual Student Student { get; set; }
        public virtual Trip Trip { get; set; }
    }
} 