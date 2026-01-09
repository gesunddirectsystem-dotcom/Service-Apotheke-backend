using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service_Apotheke.Repository.Job;
namespace ServiceApothekeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
          
    public class NotificationController : ControllerBase
    {
        private readonly IJobService _jobService;

        public NotificationController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotifications(Guid userId)
        {
            try
            {
                var result = await _jobService.GetUserNotifications(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("mark-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                var result = await _jobService.MarkNotificationAsRead(notificationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}