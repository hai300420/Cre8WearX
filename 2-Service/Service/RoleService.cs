using _2_Service.Service.IService;
using _3_Repository.IRepository;
using BusinessObject.Model;
using Repository.IRepository;
using Repository.Repository;
using Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_Service.Service
{
    //public interface IRoleService
    //{
    //    #region CRUD Category
    //    Task<IEnumerable<Role>> GetAllRoles();
    //    Task<Role> GetRoleById(int id);
    //    Task AddRole(Role role);
    //    Task UpdateRole(Role role);
    //    Task DeleteRole(int id);
    //    #endregion

    //    Task<Role> GetIdRoleByName(string name);
    //}

    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        #region CRUD Category
        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role> GetRoleById(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task AddRole(Role role)
        {
            await _roleRepository.AddAsync(role);
        }

        public async Task UpdateRole(Role role)
        {
            await _roleRepository.UpdateAsync(role);
        }

        public async Task DeleteRole(int id)
        {
            bool isRoleUsed = await _roleRepository.IsRoleUsedAsync(id);
            if (isRoleUsed)
            {
                throw new InvalidOperationException("Cannot delete this role because it is assigned to users.");
            }

            await _roleRepository.DeleteAsync(id);
        }
        #endregion


        public async Task<Role> GetIdRoleByName(string name)
        {
            return await _roleRepository.GetIdByNameAsync(name);
        }


    }
}
