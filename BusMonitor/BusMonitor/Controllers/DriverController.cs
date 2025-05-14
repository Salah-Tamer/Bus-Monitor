using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusMonitor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BusMonitor.DTOs;
using System.Security.Claims;
using AutoMapper;
using System;
using System.Linq;

namespace BusMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Driver")]
    public class DriverController : ControllerBase
    {
        private readonly BusMonitorDbContext _context;
        private readonly IMapper _mapper;

        public DriverController(BusMonitorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Driver/current-trip
        [HttpGet("current-trip")]
        public async Task<ActionResult<TripDTO>> GetCurrentTrip()
        {
            // Get the current user ID from the claims
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Get the current active trip for the driver (Active has priority, then Planned)
            var currentTrip = await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Bus)
                .Include(t => t.Supervisor)
                .Include(t => t.StudentTrips)
                    .ThenInclude(st => st.Student)
                .Where(t => t.DriverId == currentUserId && 
                      (t.Status == Status.Active || t.Status == Status.Planned))
                .OrderBy(t => t.Status == Status.Active ? 0 : 1) // Active first, then Planned
                .FirstOrDefaultAsync();
                
            if (currentTrip == null)
            {
                return NotFound("No active trips found for the driver");
            }
            
            return _mapper.Map<TripDTO>(currentTrip);
        }
        
        // GET: api/Driver/trip-history
        [HttpGet("trip-history")]
        public async Task<ActionResult<IEnumerable<TripDTO>>> GetTripHistory()
        {
            // Get the current user ID from the claims
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Get all completed trips for the driver
            var tripHistory = await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Bus)
                .Include(t => t.Supervisor)
                .Where(t => t.DriverId == currentUserId && t.Status == Status.Completed)
                .OrderByDescending(t => t.DepartureTime)
                .ToListAsync();
                
            return Ok(_mapper.Map<IEnumerable<TripDTO>>(tripHistory));
        }
        
        // GET: api/Driver/trip/{tripId}/students
        [HttpGet("trip/{tripId}/students")]
        public async Task<ActionResult<IEnumerable<StudentWithRouteDTO>>> GetTripStudents(int tripId)
        {
            // Get the current user ID from the claims
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Check if the trip belongs to the driver
            var trip = await _context.Trips
                .Include(t => t.Route)
                .FirstOrDefaultAsync(t => t.Id == tripId && t.DriverId == currentUserId);
                
            if (trip == null)
            {
                return NotFound("Trip not found or not assigned to this driver");
            }
            
            // Get all students registered for this trip with route information
            var students = await _context.StudentTrips
                .Include(st => st.Student)
                    .ThenInclude(s => s.Parent)
                .Include(st => st.Trip)
                    .ThenInclude(t => t.Route)
                .Where(st => st.TripId == tripId)
                .Select(st => new StudentWithRouteDTO
                {
                    Id = st.Student.Id,
                    Name = st.Student.Name,
                    ParentName = st.Student.Parent.Name,
                    ParentPhoneNumber = st.Student.Parent.PhoneNumber,
                    RouteStops = st.Trip.Route.Stops
                })
                .ToListAsync();
                
            return Ok(students);
        }
    }
} 