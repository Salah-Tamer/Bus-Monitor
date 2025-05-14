using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BusMonitor.Models;
using BusMonitor.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace BusMonitorTest
{
    public class AuthServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;
        private readonly List<User> _users;
        private readonly BusMonitorDbContext _context;

        public AuthServiceTests()
        {
            // Setup in-memory data
            _users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Test User",
                    Username = "testuser",
                    Password = new PasswordHasher().HashPassword("password123"),
                    PhoneNumber = "123-456-7890",
                    Role = Role.Admin
                }
            };

            // Setup in-memory database
            var options = new DbContextOptionsBuilder<BusMonitorDbContext>()
                .UseInMemoryDatabase(databaseName: $"BusMonitorTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new BusMonitorDbContext(options);
            
            // Add test user to in-memory database
            _context.Users.Add(_users[0]);
            _context.SaveChanges();

            // Setup mock Configuration
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("ThisIsMyVerySecureKeyForTestingPurposesOnly12345");
            _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            _mockConfiguration.Setup(c => c["Jwt:ExpiryInDays"]).Returns("1");

            // Create AuthService with real DbContext and mock config
            _authService = new AuthService(_context, _mockConfiguration.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
        {
            // Arrange
            string username = "testuser";
            string password = "password123";

            // Act
            var result = await _authService.AuthenticateAsync(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidUsername_ShouldReturnNull()
        {
            // Act
            var result = await _authService.AuthenticateAsync("wronguser", "password123");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnNull()
        {
            // Act
            var result = await _authService.AuthenticateAsync("testuser", "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnValidToken()
        {
            // Arrange
            var user = _users.First();

            // Act
            string token = _authService.GenerateJwtToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            // Verify token can be decoded
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.Equal("TestIssuer", jwtToken.Issuer);
            Assert.Equal("TestAudience", jwtToken.Audiences.First());
            Assert.Contains(jwtToken.Claims, c => c.Type == "role" && c.Value == Role.Admin.ToString());
            Assert.Contains(jwtToken.Claims, c => c.Type == "unique_name" && c.Value == "testuser");
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
        {
            // Act
            var result = await _authService.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var result = await _authService.GetUserByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddNewUserAsync_ShouldAddUserWithHashedPassword()
        {
            // Arrange
            var newUser = new User
            {
                Id = 2,
                Name = "New User",
                Username = "newuser",
                Password = "plainpassword",
                PhoneNumber = "987-654-3210",
                Role = Role.Driver
            };
            string originalPassword = newUser.Password;

            // Act
            await _authService.AddNewUserAsync(newUser);

            // Assert
            var addedUser = await _context.Users.FindAsync(2);
            Assert.NotNull(addedUser);
            Assert.Equal("newuser", addedUser.Username);
            Assert.NotEqual(originalPassword, addedUser.Password);
        }

        [Fact]
        public async Task UpdateUserPasswordAsync_ShouldUpdatePasswordWithHash()
        {
            // Arrange
            int userId = 1;
            string newPassword = "newpassword123";
            string originalPassword = _users.First().Password;

            // Act
            await _authService.UpdateUserPasswordAsync(userId, newPassword);

            // Assert
            var user = await _context.Users.FindAsync(userId);
            Assert.NotNull(user);
            Assert.NotEqual(originalPassword, user.Password);
            Assert.NotEqual(newPassword, user.Password); // Password should be hashed
        }
    }
} 