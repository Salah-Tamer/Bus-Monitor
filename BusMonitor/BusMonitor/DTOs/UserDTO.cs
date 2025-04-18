namespace BusMonitor.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Only used for creating users
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
} 