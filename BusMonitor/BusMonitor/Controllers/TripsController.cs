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
    [ApiController]
    [Route("api/[controller]")]
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
            var trips = await _context.Trips.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<TripDTO>>(trips));
        }

        // GET: api/Trips/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TripDTO>> GetTrip(int id)
        {
            var trip = await _context.Trips.FindAsync(id);

            if (trip == null)
            {
                return NotFound();
            }

            return _mapper.Map<TripDTO>(trip);
        }

        // PUT: api/Trips/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Supervisor,Driver")] // Only Supervisors and Drivers can update
        public async Task<IActionResult> PutTrip(int id, TripDTO tripDto)
        {
            if (id != tripDto.Id)
            {
                return BadRequest();
            }

            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            // Update trip properties
            _mapper.Map(tripDto, trip);

            _context.Entry(trip).State = EntityState.Modified;

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

        // POST: api/Trips
        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admins can Create trips
        public async Task<ActionResult<TripDTO>> PostTrip(TripDTO tripDto)
        {
            var trip = _mapper.Map<Trip>(tripDto);
            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrip), new { id = trip.Id }, _mapper.Map<TripDTO>(trip));
        }

        // DELETE: api/Trips/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admins can delete trips
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