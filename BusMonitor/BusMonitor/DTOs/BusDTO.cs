using System.ComponentModel.DataAnnotations;

namespace BusMonitor.DTOs
{
    public class BusDTO
    {
        public int Id { get; set; }
        
        public string Number { get; set; }
        
        public int Capacity { get; set; }
    }
} 