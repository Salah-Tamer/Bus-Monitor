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
    }
} 