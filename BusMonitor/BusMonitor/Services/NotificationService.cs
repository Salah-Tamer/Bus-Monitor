using BusMonitor.Models;
using BusMonitor.DTOs;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace BusMonitor.Services
{
    public interface INotificationService
    {
        Task CreateBehaviorNotificationAsync(StudentReport report);
        Task CreateDelayNotificationAsync(DelayReport report);
        Task<IEnumerable<NotificationDTO>> GetParentNotificationsAsync(int parentId);
        Task MarkNotificationAsReadAsync(int notificationId, int parentId);
    }

    public class NotificationService : INotificationService
    {
        private readonly BusMonitorDbContext _context;
        private readonly IMapper _mapper;

        public NotificationService(BusMonitorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateBehaviorNotificationAsync(StudentReport report)
        {
            var student = await _context.Students
                .Include(s => s.Parent)
                .FirstOrDefaultAsync(s => s.Id == report.StudentId);

            if (student?.Parent == null) return;

            var notification = new Notification
            {
                ParentId = student.Parent.Id,
                StudentId = student.Id,
                TripId = report.TripId,
                Title = "Behavior Report",
                Message = $"Behavior report for {student.Name}: {(report.IsPresent ? "Present" : "Absent")}. Notes: {report.BehaviorNotes}",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task CreateDelayNotificationAsync(DelayReport report)
        {
            var trip = await _context.Trips
                .Include(t => t.StudentTrips)
                    .ThenInclude(st => st.Student)
                        .ThenInclude(s => s.Parent)
                .FirstOrDefaultAsync(t => t.Id == report.TripId);

            if (trip?.StudentTrips == null) return;

            foreach (var studentTrip in trip.StudentTrips)
            {
                if (studentTrip.Student?.Parent == null) continue;

                var notification = new Notification
                {
                    ParentId = studentTrip.Student.Parent.Id,
                    StudentId = studentTrip.Student.Id,
                    TripId = report.TripId,
                    Title = "Trip Delay",
                    Message = $"Trip delay notification: {report.DelayReason}. Additional notes: {report.AdditionalNotes}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<NotificationDTO>> GetParentNotificationsAsync(int parentId)
        {
            var notifications = await _context.Notifications
                .Include(n => n.Student)
                .Where(n => n.ParentId == parentId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
        }

        public async Task MarkNotificationAsReadAsync(int notificationId, int parentId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.ParentId == parentId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
} 