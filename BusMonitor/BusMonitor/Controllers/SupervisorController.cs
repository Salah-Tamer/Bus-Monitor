using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusMonitor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BusMonitor.DTOs;
using System.Security.Claims;
using AutoMapper;
using BusMonitor.Services;

namespace BusMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : ControllerBase
    {
        private readonly BusMonitorDbContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public SupervisorController(
            BusMonitorDbContext context, 
            IMapper mapper,
            INotificationService notificationService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        // GET: api/Supervisor/assigned-students
        [HttpGet("assigned-students")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAssignedStudents()
        {
            var supervisorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var trips = await _context.Trips
                .Include(t => t.StudentTrips)
                    .ThenInclude(st => st.Student)
                .Where(t => t.SupervisorId == supervisorId && t.Status == Status.Active)
                .ToListAsync();

            var students = trips
                .SelectMany(t => t.StudentTrips.Select(st => st.Student))
                .Distinct()
                .ToList();

            return Ok(_mapper.Map<IEnumerable<StudentDTO>>(students));
        }

        // GET: api/Supervisor/active-trips
        [HttpGet("active-trips")]
        public async Task<ActionResult<IEnumerable<TripDTO>>> GetActiveTrips()
        {
            var supervisorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var trips = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Route)
                .Include(t => t.StudentTrips)
                    .ThenInclude(st => st.Student)
                .Where(t => t.SupervisorId == supervisorId && t.Status == Status.Active)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<TripDTO>>(trips));
        }

        // POST: api/Supervisor/behavior-reports
        [HttpPost("behavior-reports")]
        public async Task<ActionResult<StudentReportDTO>> CreateBehaviorReport(CreateStudentReportDTO reportDto)
        {
            var supervisorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Get the supervisor's active trip
            var activeTrip = await _context.Trips
                .FirstOrDefaultAsync(t => t.SupervisorId == supervisorId && t.Status == Status.Active);

            if (activeTrip == null)
            {
                return BadRequest("You don't have any active trips assigned");
            }

            // Verify the student is assigned to this trip
            var studentTrip = await _context.StudentTrips
                .FirstOrDefaultAsync(st => st.StudentId == reportDto.StudentId && st.TripId == activeTrip.Id);

            if (studentTrip == null)
            {
                return BadRequest("Student is not assigned to your active trip");
            }

            // Validate behavior category
            if (!Enum.TryParse<BehaviorCategory>(reportDto.BehaviorCategory, out var behaviorCategory))
            {
                return BadRequest("Invalid behavior category. Must be either 'Positive' or 'Negative'");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var report = _mapper.Map<StudentReport>(reportDto);
                report.SupervisorId = supervisorId;
                report.TripId = activeTrip.Id; // Use the active trip ID
                report.ReportDate = DateTime.UtcNow;

                _context.StudentReports.Add(report);
                await _context.SaveChangesAsync();

                // Create notification for parent
                var student = await _context.Students
                    .Include(s => s.Parent)
                    .FirstOrDefaultAsync(s => s.Id == reportDto.StudentId);

                if (student != null)
                {
                    var notification = new Notification
                    {
                        ParentId = student.ParentId,
                        StudentId = student.Id,
                        TripId = activeTrip.Id,
                        Title = $"Behavior Report - {reportDto.BehaviorCategory}",
                        Message = $"Behavior Report: {reportDto.BehaviorCategory} - {reportDto.BehaviorNotes}",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                var mappedReport = _mapper.Map<StudentReportDTO>(report);
                return CreatedAtAction(nameof(GetBehaviorReport), new { id = report.Id }, mappedReport);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // GET: api/Supervisor/behavior-reports/{id}
        [HttpGet("behavior-reports/{id}")]
        public async Task<ActionResult<StudentReportDTO>> GetBehaviorReport(int id)
        {
            var supervisorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var report = await _context.StudentReports
                .Include(r => r.Student)
                .Include(r => r.Trip)
                .FirstOrDefaultAsync(r => r.Id == id && r.SupervisorId == supervisorId);

            if (report == null)
            {
                return NotFound();
            }

            return _mapper.Map<StudentReportDTO>(report);
        }

        // GET: api/Supervisor/behavior-reports
        [HttpGet("behavior-reports")]
        public async Task<ActionResult<IEnumerable<StudentReportDTO>>> GetBehaviorReports([FromQuery] int? tripId = null)
        {
            var supervisorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var query = _context.StudentReports
                .Include(r => r.Student)
                .Include(r => r.Trip)
                .Where(r => r.SupervisorId == supervisorId);

            if (tripId.HasValue)
            {
                query = query.Where(r => r.TripId == tripId.Value);
            }

            var reports = await query.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<StudentReportDTO>>(reports));
        }

        // POST: api/Supervisor/delay-reports
        [HttpPost("delay-reports")]
        public async Task<ActionResult<DelayReportDTO>> CreateDelayReport(CreateDelayReportDTO reportDto)
        {
            var supervisorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Verify the supervisor is assigned to this trip
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == reportDto.TripId && t.SupervisorId == supervisorId);

            if (trip == null)
            {
                return Forbid("You are not assigned to this trip");
            }

            var report = new DelayReport
            {
                TripId = reportDto.TripId,
                SupervisorId = supervisorId,
                ReportDate = DateTime.UtcNow,
                DelayReason = reportDto.DelayReason,
                AdditionalNotes = reportDto.AdditionalNotes
            };

            _context.DelayReports.Add(report);
            await _context.SaveChangesAsync();

            // Send notifications to all parents of students in the trip
            await _notificationService.CreateDelayNotificationAsync(report);

            var mappedReport = _mapper.Map<DelayReportDTO>(report);
            return CreatedAtAction(nameof(GetDelayReport), new { id = report.Id }, mappedReport);
        }

        // GET: api/Supervisor/delay-reports/{id}
        [HttpGet("delay-reports/{id}")]
        public async Task<ActionResult<DelayReportDTO>> GetDelayReport(int id)
        {
            var supervisorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var report = await _context.DelayReports
                .Include(r => r.Trip)
                .FirstOrDefaultAsync(r => r.Id == id && r.SupervisorId == supervisorId);

            if (report == null)
            {
                return NotFound();
            }

            return _mapper.Map<DelayReportDTO>(report);
        }

        // GET: api/Supervisor/delay-reports
        [HttpGet("delay-reports")]
        public async Task<ActionResult<IEnumerable<DelayReportDTO>>> GetDelayReports([FromQuery] int? tripId = null)
        {
            var supervisorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var query = _context.DelayReports
                .Include(r => r.Trip)
                .Where(r => r.SupervisorId == supervisorId);

            if (tripId.HasValue)
            {
                query = query.Where(r => r.TripId == tripId.Value);
            }

            var reports = await query.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<DelayReportDTO>>(reports));
        }
    }
} 