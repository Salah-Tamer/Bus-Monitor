using Microsoft.AspNetCore.Mvc;
using BusMonitor.Services;
using System.Threading.Tasks;
using BusMonitor.DTOs;
using AutoMapper;

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
            var user = await _authService.AuthenticateAsync(model.Username, model.Password);
            
            if (user == null)
                return Unauthorized("Invalid username or password");
            
            var token = _authService.GenerateJwtToken(user);
            var userDto = _mapper.Map<UserDTO>(user);
            
            return Ok(new LoginResponseDTO 
            { 
                Token = token,
                User = userDto
            });
        }
    }
} 