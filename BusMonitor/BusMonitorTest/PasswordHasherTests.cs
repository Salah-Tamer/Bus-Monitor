using Xunit;
using System;
using BusMonitor.Models;

namespace BusMonitorTest
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher;

        public PasswordHasherTests()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void HashPassword_ShouldReturnHashedPassword()
        {
            // Arrange
            string password = "TestPassword123!";

            // Act
            string hashedPassword = _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEqual(password, hashedPassword);
            Assert.Contains(".", hashedPassword); // Should contain salt and hash separated by a dot
            var parts = hashedPassword.Split('.');
            Assert.Equal(2, parts.Length);
        }

        [Fact]
        public void VerifyHashedPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            string password = "TestPassword123!";
            string hashedPassword = _passwordHasher.HashPassword(password);

            // Act
            bool result = _passwordHasher.VerifyHashedPassword(hashedPassword, password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyHashedPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            string password = "TestPassword123!";
            string wrongPassword = "WrongPassword123!";
            string hashedPassword = _passwordHasher.HashPassword(password);

            // Act
            bool result = _passwordHasher.VerifyHashedPassword(hashedPassword, wrongPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyHashedPassword_WithInvalidFormat_ShouldReturnFalse()
        {
            // Arrange
            string invalidHashedPassword = "InvalidFormat";

            // Act
            bool result = _passwordHasher.VerifyHashedPassword(invalidHashedPassword, "anypassword");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HashNewUserPassword_ShouldUpdateUserPassword()
        {
            // Arrange
            var user = new User { Password = "TestPassword123!" };
            string originalPassword = user.Password;

            // Act
            _passwordHasher.HashNewUserPassword(user);

            // Assert
            Assert.NotEqual(originalPassword, user.Password);
            Assert.Contains(".", user.Password);
        }
    }
}