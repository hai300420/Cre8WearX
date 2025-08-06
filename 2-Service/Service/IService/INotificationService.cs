using BusinessObject.Model;
using BusinessObject.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _2_Service.Service.IService
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
}
