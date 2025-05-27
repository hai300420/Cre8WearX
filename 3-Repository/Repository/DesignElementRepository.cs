using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3_Repository.IRepository;
using BusinessObject.Model;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace _3_Repository.Repository
{
    public class DesignElementRepository : GenericRepository<DesignElement>, IDesignElementRepository
    {
        private readonly ClothesCusShopContext _context;

        public DesignElementRepository(ClothesCusShopContext context) : base(context) { _context = context; }

        public async Task<IEnumerable<DesignElement>> GetAllAsync()
        {
            return await _context.DesignElements
                .Include(de => de.DesignArea)
                .Include(de => de.CustomizeProduct)
                .ToListAsync();
        }

        public async Task<DesignElement> GetByIdAsync(int id)
        {
            return await _context.DesignElements
                .Include(de => de.DesignArea)
                .Include(de => de.CustomizeProduct)
                .FirstOrDefaultAsync(de => de.DesignElementId == id);
        }

        public async Task AddAsync(DesignElement designElement)
        {
            await _context.DesignElements.AddAsync(designElement);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DesignElement designElement)
        {
            _context.DesignElements.Update(designElement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var designElement = await _context.DesignElements.FindAsync(id);
            if (designElement != null)
            {
                _context.DesignElements.Remove(designElement);
                await _context.SaveChangesAsync();
            }
        }
    }

}
