using Microsoft.AspNetCore.Mvc;
using BusMonitor.Services;
using System.Threading.Tasks;
using BusMonitor.DTOs;
using AutoMapper;
using System;

namespace BusMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest("Username and password are required");
                }
                
                var user = await _authService.AuthenticateAsync(model.Username, model.Password);
                
                if (user == null)
                    return Unauthorized("Invalid username or password");
                
                var token = _authService.GenerateJwtToken(user);
                
                if (string.IsNullOrEmpty(token))
                {
                    return StatusCode(500, "Failed to generate JWT token");
                }
                
                var userDto = _mapper.Map<UserDTO>(user);
                
                return Ok(new LoginResponseDTO 
                { 
                    Token = token,
                    User = userDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during login: {ex.Message}");
            }
        }
    }
} 