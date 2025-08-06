using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_Service.Service.IService
{
    public interface IRoleService
    {
        #region CRUD Category
        Task<IEnumerable<Role>> GetAllRoles();
        Task<Role> GetRoleById(int id);
        Task AddRole(Role role);
        Task UpdateRole(Role role);
        Task DeleteRole(int id);
        #endregion

        Task<Role> GetIdRoleByName(string name);
    }

}
