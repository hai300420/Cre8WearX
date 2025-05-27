using _3_Repository.Repository;
using BusinessObject.Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Repository.IRepository
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        #region CRUD Notifications
        Task<IEnumerable<Notification>> GetAllAsync();
        Task<Notification> GetByIdAsync(int id);
        Task AddAsync(Notification notification);
        Task DeleteAsync(int id);
        Task UpdateAsync(Notification notification);
        #endregion


        Task AddRangeAsync(List<Notification> notifications);
        Task<List<Notification>> GetNotificationsByRoleAsync(string role, DateTime? fromDate = null, DateTime? toDate = null);
        Task DeleteNotificationsByRoleAndTimeRangeAsync(string role, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
    }
}
