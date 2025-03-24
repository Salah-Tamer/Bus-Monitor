using Microsoft.AspNetCore.Mvc;
using BusMonitor.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BusMonitor.DTOs;
using System.Security.Claims;

namespace BusMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Parent")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetNotifications()
        {
            var parentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var notifications = await _notificationService.GetParentNotificationsAsync(parentId);
            return Ok(notifications);
        }

        // PUT: api/Notifications/5/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var parentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _notificationService.MarkNotificationAsReadAsync(id, parentId);
            return NoContent();
        }
    }
} 