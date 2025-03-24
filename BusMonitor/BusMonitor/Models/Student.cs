using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusMonitor.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Grade { get; set; }

        public int? Absence { get; set; }

        [Required]
        [ForeignKey("Parent")]
        public int ParentId { get; set; }

        // Navigation properties
        public virtual User Parent { get; set; }
        public virtual ICollection<StudentTrip> StudentTrips { get; set; }
        public virtual ICollection<StudentReport> BehaviorReports { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        public Student()
        {
            StudentTrips = new HashSet<StudentTrip>();
            BehaviorReports = new HashSet<StudentReport>();
            Notifications = new HashSet<Notification>();
        }
    }
}
