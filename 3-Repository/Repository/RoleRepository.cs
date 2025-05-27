using BusinessObject.Model;
using Repository.IRepository;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data;
using _3_Repository.IRepository;

namespace _3_Repository.Repository
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ClothesCusShopContext context) : base(context)
        {
            _context = context;
        }

        #region CRUD Category
        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }
        public async Task<Role> GetByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }


        public async Task AddAsync(Role role)
        {
            _context.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {

            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }
        #endregion



        public async Task<Role> GetIdByNameAsync(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.RoleName.ToLower() == name.ToLower());
        }

        public async Task<string> GetNameByIdAsync(int id)
        {
            var roleName = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == id);
            return roleName.RoleName;
        }

        public async Task<bool> IsRoleUsedAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.RoleId == id);
        }



    }
}
