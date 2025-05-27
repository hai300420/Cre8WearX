using _3_Repository.IRepository;
using _3_Repository.Repository;
using BusinessObject;
using BusinessObject.Model;
using BusinessObject.ResponseDTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _2_Service.Service
{
    public interface INotificationService
    {
        #region CRUD Notifications
        Task<IEnumerable<Notification>> GetAllNotifications();
        Task<Notification> GetNotificationById(int id);
        Task<ResponseDTO> AddNotification(PostNotificationDTO notificationDto);
        Task<ResponseDTO> UpdateNotification(int id, PutNotificationDTO notificationDto);
        Task<ResponseDTO> DeleteNotification(int id);
        #endregion


        Task<ResponseDTO> SendNotificationToSpecificRole(string subject, string message, string role);
        Task<ResponseDTO> GetAllNotificationsRoleAndTimeRange(string role, DateTime? fromDate, DateTime? toDate);
        Task<ResponseDTO> DeleteNotificationsRoleAndTimeRange(string role, DateTime? fromDate, DateTime? toDate);
    }
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        #region CRUD Notifications
        public async Task<IEnumerable<Notification>> GetAllNotifications()
        {
            var currentUserRole = _httpContextAccessor.HttpContext.User.Claims
                                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var currentUserIdClaim = _httpContextAccessor.HttpContext.User.Claims
                                .FirstOrDefault(c => c.Type == "User_Id")?.Value;

            if (string.IsNullOrEmpty(currentUserRole) || string.IsNullOrEmpty(currentUserIdClaim))
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            int currentUserId = int.Parse(currentUserIdClaim);

            if (currentUserRole.ToLower() == "admin" || currentUserRole.ToLower() == "staff")
            {
                // Admin & Staff can see all notifications
                return await _notificationRepository.GetAllAsync();
            }
            else
            {
                // Members can only see their own notifications
                return await _notificationRepository.GetNotificationsByUserIdAsync(currentUserId);
            }
        }

        public async Task<Notification> GetNotificationById(int id)
        {
            return await _notificationRepository.GetByIdAsync(id);
        }

        public async Task<ResponseDTO> AddNotification(PostNotificationDTO notificationDto)
        {
            var notification = new Notification
            {
                UserId = notificationDto.UserId,
                Subject = notificationDto.Subject,
                Message = notificationDto.Message,
                CreatedDate = DateTime.Now,
                IsRead = false,
            };

            await _notificationRepository.AddAsync(notification);
            return new ResponseDTO(Const.SUCCESS_READ_CODE, "Notification created successfully", notification);
        }

        public async Task<ResponseDTO> UpdateNotification(int id, PutNotificationDTO notificationDto)
        {
            var existingNotification = await _notificationRepository.GetByIdAsync(id);
            if (existingNotification == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Notification not found");
            }

            // Update only allowed properties
            existingNotification.Subject = notificationDto.Subject;
            existingNotification.Message = notificationDto.Message;

            await _notificationRepository.UpdateAsync(existingNotification);
            return new ResponseDTO(Const.SUCCESS_READ_CODE, "Notification updated successfully");
        }

        public async Task<ResponseDTO> DeleteNotification(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Notification not found");
            }

            await _notificationRepository.DeleteAsync(id);
            return new ResponseDTO(Const.SUCCESS_READ_CODE, "Notification deleted successfully");
        }
        #endregion




        public async Task<ResponseDTO> SendNotificationToSpecificRole(string subject, string message, string role)
        {
            // Get users who have the specified role
            var targetUsers = await _userRepository.GetUsersByRoleAsync(role);

            if (!targetUsers.Any())
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, $"No users found with the role {role}.");
            }

            // Create notifications for each user
            var notifications = targetUsers.Select(user => new Notification
            {
                UserId = user.UserId,
                Subject = subject,
                Message = message,
                CreatedDate = DateTime.UtcNow,
                IsRead = false
            }).ToList();

            // Save notifications in bulk
            await _notificationRepository.AddRangeAsync(notifications);

            return new ResponseDTO(Const.SUCCESS_READ_CODE, $"Notifications sent to {targetUsers.Count} users with role {role}.");
        }

        public async Task<ResponseDTO> GetAllNotificationsRoleAndTimeRange(string role, DateTime? fromDate, DateTime? toDate)
        {
            var notifications = await _notificationRepository.GetNotificationsByRoleAsync(role, fromDate, toDate);

            if (!notifications.Any())
                return new ResponseDTO(Const.FAIL_READ_CODE, "No notifications found.");

            return new ResponseDTO(Const.SUCCESS_READ_CODE, "Notifications deleted successfully.", notifications);
        }

        public async Task<ResponseDTO> DeleteNotificationsRoleAndTimeRange(string role, DateTime? fromDate, DateTime? toDate)
        {
            await _notificationRepository.DeleteNotificationsByRoleAndTimeRangeAsync(role, fromDate, toDate);
            return new ResponseDTO(Const.SUCCESS_READ_CODE, "Notifications deleted successfully.");
        }


    }
}
