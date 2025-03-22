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

namespace BusMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // This requires authentication for all endpoints in this controller
    public class UsersController : ControllerBase
    {
        private readonly BusMonitorDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(BusMonitorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        public async Task<IActionResult> PutUser(int id, UserDTO userDto)
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
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            
            // Ensure password is set directly without hashing
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.Password = userDto.Password;
            }
            
            var createdUser = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Entity.Id }, _mapper.Map<UserDTO>(createdUser.Entity));
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
    }
} 