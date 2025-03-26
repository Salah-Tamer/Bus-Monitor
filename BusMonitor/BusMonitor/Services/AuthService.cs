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
        Task<User> AuthenticateAsync(string username, string password);
        string GenerateJwtToken(User user);
        Task<User> GetUserByIdAsync(int id);
        Task AddNewUserAsync(User user);
        Task HashAllPasswordsInUserTableAsync();
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

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

            // Return null if user not found or password doesn't match
            //if (user == null || !_passwordHasher.VerifyPassword(user.Password, password))
            //    return null;

            return user;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:ExpiryInDays"] ?? "7")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddNewUserAsync(User user)
        {
            user.Password = _passwordHasher.HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task HashAllPasswordsInUserTableAsync()
        {
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                if (!user.Password.StartsWith("hashed:"))
                {
                    user.Password = "hashed:" + _passwordHasher.HashPassword(user.Password);
                }
            }
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
