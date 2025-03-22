namespace BusMonitor.DTOs
{

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public UserDTO User { get; set; }
    }
} 