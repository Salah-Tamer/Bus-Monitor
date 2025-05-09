using BusMonitor.Models;
using BusMonitor.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusMonitor.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        string GenerateJwtToken(User user);
        Task<User?> GetUserByIdAsync(int id);
        Task AddNewUserAsync(User user);
        Task UpdateUserPasswordAsync(int userId, string newPassword);
    }

    public class AuthService : IAuthService
    {
        private readonly BusMonitorDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher _passwordHasher;

        public AuthService(BusMonitorDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher();
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

            // Return null if user not found
            if (user == null)
                return null;
                
            // Temporarily just compare plain text passwords
            if (user.Password != password)
                return null;
                
            return user;
        }

        public string GenerateJwtToken(User user)
        {
            try
            {
                var key = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException(nameof(key), "JWT key is not configured.");
                }
                
                var issuer = _configuration["Jwt:Issuer"];
                if (string.IsNullOrEmpty(issuer))
                {
                    throw new ArgumentNullException(nameof(issuer), "JWT issuer is not configured.");
                }
                
                var audience = _configuration["Jwt:Audience"];
                if (string.IsNullOrEmpty(audience))
                {
                    throw new ArgumentNullException(nameof(audience), "JWT audience is not configured.");
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var keyBytes = Encoding.ASCII.GetBytes(key);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:ExpiryInDays"] ?? "7")),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = issuer,
                    Audience = audience
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating token: {ex.Message}");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddNewUserAsync(User user)
        {
            _passwordHasher.HashNewUserPassword(user);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserPasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(newPassword);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
