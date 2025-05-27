using _2_Service.Service;
using BusinessObject;
using BusinessObject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _1_SPR25_SWD392_ClothingCustomization.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        #region CRUD notification
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAll()
        {
            return Ok(await _notificationService.GetAllNotifications());
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Notification>> GetById(int id)
        {
            var notification = await _notificationService.GetNotificationById(id);
            if (notification == null)
                return NotFound();
            return Ok(notification);
        }

        [HttpPost]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Create([FromBody] PostNotificationDTO notificationDto)
        {
            var response = await _notificationService.AddNotification(notificationDto);
            return StatusCode(response.Status == Const.SUCCESS_READ_CODE ? 200 : 400, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Update(int id, [FromBody] PutNotificationDTO notificationDto)
        {
            var response = await _notificationService.UpdateNotification(id, notificationDto);
            if (response.Status == Const.FAIL_READ_CODE)
                return NotFound(response);
            return Ok(response);

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, staff")]
        public async Task<ActionResult> Delete(int id)
        {
            var response = await _notificationService.DeleteNotification(id);
            if (response.Status == Const.FAIL_READ_CODE)
                return NotFound(response);
            return Ok(response);
        }
        #endregion



        [HttpPost("bulk")]
        [Authorize(Roles = "admin, staff")]
        public async Task<IActionResult> SendNotificationToRole([FromBody] NotificationRoleDTO notificationDto)
        {
            var response = await _notificationService.SendNotificationToSpecificRole(notificationDto.Subject, notificationDto.Message, notificationDto.RoleName);
            return StatusCode(response.Status == Const.SUCCESS_READ_CODE ? 200 : 400, response);
        }

        [HttpGet("role-and-time")]
        [Authorize]
        public async Task<IActionResult> GetAllNotificationsTimeRangeAndRole(string role, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var response = await _notificationService.GetAllNotificationsRoleAndTimeRange(role, fromDate, toDate);
            return Ok(response);
        }

        [HttpDelete]
        [Authorize(Roles = "admin, staff")]
        public async Task<IActionResult> DeleteByRoleOrTime(string role, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var response = await _notificationService.DeleteNotificationsRoleAndTimeRange(role, fromDate, toDate);
            return Ok(response);
        }



    }
}
