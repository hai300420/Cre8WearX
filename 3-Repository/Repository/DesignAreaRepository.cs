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
    public class DesignAreaRepository : GenericRepository<DesignArea>, IDesignAreaRepository
    {
        private readonly ClothesCusShopContext _context;

        public DesignAreaRepository(ClothesCusShopContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddDesignAreaAsync(DesignArea designArea)
        {
            _context.Add(designArea);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDesignAreaAsync(int id)
        {
            var designArea = await _context.DesignAreas.FindAsync(id);
            if (designArea != null)
            {
                _context.DesignAreas.Remove(designArea);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DesignArea>> GetAllDesignAreaAsync()
        {
            return await _context.DesignAreas
        .Include(cp => cp.DesignElements)  
        .Include(cp => cp.Product)    
        .ToListAsync();
        }

        public async Task<DesignArea> GetDesignAreaByIdAsync(int id)
        {
            return await _context.DesignAreas
       .Include(cp => cp.DesignElements)
       .Include(cp => cp.Product)
       .FirstOrDefaultAsync(cp => cp.DesignAreaId == id);
        }

        public async Task UpdateDesignAreaAsync(DesignArea designArea)
        {
            _context.Update(designArea);
            await _context.SaveChangesAsync();
        }
    }

}
