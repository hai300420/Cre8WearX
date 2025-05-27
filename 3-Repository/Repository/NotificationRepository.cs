using _3_Repository.IRepository;
using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Repository.Repository
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {

        public NotificationRepository(ClothesCusShopContext context) : base(context)
        {
            _context = context;
        }

        #region CRUD Notification
        public async Task AddAsync(Notification notification)
        {
            _context.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }


        public async Task<Notification> GetByIdAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        #endregion


        public async Task AddRangeAsync(List<Notification> notifications)
        {
            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetNotificationsByRoleAsync(string role, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var usersWithRole = await _context.Users
                .Where(u => u.Role.RoleName.ToLower() == role.ToLower())
                .Select(u => u.UserId)
                .ToListAsync();

            var query = _context.Notifications.Where(n => usersWithRole.Contains(n.UserId));

            if (fromDate.HasValue)
                query = query.Where(n => n.CreatedDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(n => n.CreatedDate <= toDate.Value);

            return await query.ToListAsync();
        }

        public async Task DeleteNotificationsByRoleAndTimeRangeAsync(string role, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var usersWithRole = await _context.Users
                .Where(u => u.Role.RoleName.ToLower() == role.ToLower())
                .Select(u => u.UserId)
                .ToListAsync();

            var query = _context.Notifications.Where(n => usersWithRole.Contains(n.UserId));

            if (fromDate.HasValue)
                query = query.Where(n => n.CreatedDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(n => n.CreatedDate <= toDate.Value);

            _context.Notifications.RemoveRange(query);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();
        }


    }
}
