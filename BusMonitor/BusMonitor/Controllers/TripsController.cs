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

namespace BusMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TripsController : ControllerBase
    {
        private readonly BusMonitorDbContext _context;
        private readonly IMapper _mapper;

        public TripsController(BusMonitorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Trips
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDTO>>> GetTrips()
        {
            var trips = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Route)
                .Include(t => t.Driver)
                .Include(t => t.Supervisor)
                .Include(t => t.Admin)
                .Include(t => t.StudentTrips)
                    .ThenInclude(st => st.Student)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<TripDTO>>(trips));
        }

        // GET: api/Trips/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TripDTO>> GetTrip(int id)
        {
            var trip = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Route)
                .Include(t => t.Driver)
                .Include(t => t.Supervisor)
                .Include(t => t.Admin)
                .Include(t => t.StudentTrips)
                    .ThenInclude(st => st.Student)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
            {
                return NotFound();
            }

            return _mapper.Map<TripDTO>(trip);
        }

        // POST: api/Trips
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TripDTO>> CreateTrip(CreateTripDTO tripDto)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Check if the supervisor is already assigned to an active trip
            if (tripDto.SupervisorId.HasValue)
            {
                var existingActiveTrip = await _context.Trips
                    .AnyAsync(t => t.SupervisorId == tripDto.SupervisorId && t.Status.ToString() == Status.Active.ToString());

                if (existingActiveTrip)
                {
                    return BadRequest("The supervisor is already assigned to an active trip");
                }
            }

            var trip = _mapper.Map<Trip>(tripDto);
            trip.AdminId = adminId;
            trip.Status = Status.Planned;

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            var createdTrip = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Route)
                .Include(t => t.Driver)
                .Include(t => t.Supervisor)
                .Include(t => t.Admin)
                .FirstOrDefaultAsync(t => t.Id == trip.Id);

            return CreatedAtAction(nameof(GetTrip), new { id = trip.Id }, _mapper.Map<TripDTO>(createdTrip));
        }

        // PUT: api/Trips/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTrip(int id, UpdateTripDTO tripDto)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            // Check if the supervisor is already assigned to an active trip (excluding the current trip)
            if (tripDto.SupervisorId.HasValue && tripDto.Status == "Active")
            {
                var existingActiveTrip = await _context.Trips
                    .AnyAsync(t => t.SupervisorId == tripDto.SupervisorId && 
                                 t.Status.ToString() == "Active" && 
                                 t.Id != id);

                if (existingActiveTrip)
                {
                    return BadRequest("The supervisor is already assigned to an active trip");
                }
            }

            // Update trip properties
            trip.BusId = tripDto.BusId;
            trip.RouteId = tripDto.RouteId;
            trip.DriverId = tripDto.DriverId;
            trip.SupervisorId = tripDto.SupervisorId;
            trip.Status = Enum.Parse<Status>(tripDto.Status);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripExists(id))
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

        // DELETE: api/Trips/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.Id == id);
        }
    }
} 