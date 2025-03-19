using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusMonitor.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Grade { get; set; }

        public int? Absence { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }

        // Navigation properties
        public virtual User Parent { get; set; }
        public virtual ICollection<StudentTrip> StudentTrips { get; set; }

        public Student()
        {
            StudentTrips = new HashSet<StudentTrip>();
        }
    }
}
