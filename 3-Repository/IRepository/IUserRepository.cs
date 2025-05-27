using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Model;
using Repository;

namespace _3_Repository.IRepository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        #region CRUD User
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task DeleteAsync(int id);
        Task UpdateAsync(User user);
        #endregion

        Task SoftDeleteAsync(int id);
        Task RecoverAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetUsersByRoleAsync(string roleName);

    }
}
