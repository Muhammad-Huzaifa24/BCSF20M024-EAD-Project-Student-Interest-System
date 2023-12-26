using EAD_Project_II.Models;
using EAD_Project_II.Services;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Project_II.Controllers
{
    public class ActivityLogController : Controller
    {
        private readonly IActivityLoggerService _activityLoggerService;

        public ActivityLogController(IActivityLoggerService activityLoggerService)
        {
            _activityLoggerService = activityLoggerService;
        }

        [HttpPost("log")]
        public IActionResult LogUserActivity(ActivityLogModel activityLogModel)
        {
            try
            {
                _activityLoggerService.LogActivity(activityLogModel);
                return Ok("Activity logged successfully");
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, $"Error logging activity: {ex.Message}");
            }
        }

        [HttpGet("last30days/{userId}")]
        public IActionResult GetLast30DaysActivity(int userId)
        {
            try
            {
                var last30DaysActivity = _activityLoggerService.GetLast30DaysActivity(userId);
                return Ok(last30DaysActivity);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, $"Error fetching last 30 days activity: {ex.Message}");
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
