using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusMonitor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BusMonitor.Services;
using AutoMapper;
using BusMonitor.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace BusMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // This requires authentication for all endpoints in this controller
    public class UsersController : ControllerBase
    {
        private readonly BusMonitorDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly PasswordHasher _passwordHasher;

        public UsersController(BusMonitorDbContext context, IMapper mapper, IAuthService authService, PasswordHasher passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
            _passwordHasher = passwordHasher;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Admin")] // Only Admins can get all users
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            // Check if user is requesting their own profile or is an admin
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var isAdmin = User.IsInRole("Admin");
            
            if (id != currentUserId && !isAdmin)
            {
                return Forbid();
            }
            
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return _mapper.Map<UserDTO>(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, CreateUpdateUserDTO userDto)
        {
            // Check if user is updating their own profile or is an admin
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var isAdmin = User.IsInRole("Admin");
            
            if (id != currentUserId && !isAdmin)
            {
                return Forbid();
            }
            
            if (id != userDto.Id)
            {
                return BadRequest();
            }
            
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            // Update user properties but don't change the password
            user.Name = userDto.Name;
            user.Username = userDto.Username;
            user.PhoneNumber = userDto.PhoneNumber;
            
            // Only admin can change roles
            if (isAdmin && !string.IsNullOrEmpty(userDto.Role) && Enum.TryParse<Role>(userDto.Role, out var role))
            {
                user.Role = role;
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                await _authService.UpdateUserPasswordAsync(id, userDto.Password);
                return NoContent();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admins can create users
        public async Task<ActionResult<UserDTO>> PostUser(CreateUpdateUserDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);

            // Ensure password is set
            if (string.IsNullOrEmpty(userDto.Password))
            {
                return BadRequest("Password is required");
            }

            // Use AuthService to add the new user and hash the password
            await _authService.AddNewUserAsync(user);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, _mapper.Map<UserDTO>(user));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only admins can delete users
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // GET: api/Users/drivers
        [HttpGet("drivers")]
        [Authorize(Roles = "Admin")] // Only Admins can get all drivers
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetDrivers()
        {
            var drivers = await _context.Users
                .Where(u => u.Role == Role.Driver)
                .ToListAsync();
                
            return Ok(_mapper.Map<IEnumerable<UserDTO>>(drivers));
        }

        // GET: api/Users/students
        [HttpGet("students")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudents()
        {
            var students = await _context.Students
                .Include(s => s.Parent)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<StudentDTO>>(students));
        }

        // GET: api/Users/buses
        [HttpGet("buses")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BusDTO>>> GetAllBuses()
        {
            var buses = await _context.Buses
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<BusDTO>>(buses));
        }

        // GET: api/Users/routes
        [HttpGet("routes")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RouteDTO>>> GetAllRoutes()
        {
            var routes = await _context.Routes
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<RouteDTO>>(routes));
        }
    }
} 