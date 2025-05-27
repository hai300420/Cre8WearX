using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3_Repository.IRepository;
using BusinessObject.Model;
using Microsoft.EntityFrameworkCore;
using Repository;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _3_Repository.Repository
{
    public class CustomizeProductRepository : GenericRepository<CustomizeProduct>, ICustomizeProductRepository
    {
        private readonly ClothesCusShopContext _context;

        public CustomizeProductRepository(ClothesCusShopContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(CustomizeProduct customizeProduct)
        {
            await _context.CustomizeProducts.AddAsync(customizeProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customizeProduct = await _context.CustomizeProducts.FindAsync(id);
            if (customizeProduct != null)
            {
                _context.CustomizeProducts.Remove(customizeProduct);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(CustomizeProduct customizeProduct)
        {
            _context.CustomizeProducts.Update(customizeProduct);
            await _context.SaveChangesAsync();
        }

        Task ICustomizeProductRepository.UpdateAsync(CustomizeProduct customizeProduct)
        {
            _context.CustomizeProducts.Update(customizeProduct);
            return _context.SaveChangesAsync();
        }
        public async Task<CustomizeProduct> GetByIdAsync(int id)
        {
            return await _context.CustomizeProducts
       .Include(cp => cp.DesignElements)
       .Include(cp => cp.Product)
       .Include(cp => cp.User)
       .FirstOrDefaultAsync(cp => cp.CustomizeProductId == id);
        }

        public async Task<IEnumerable<CustomizeProduct>> GetAllAsync()
        {
            return await _context.CustomizeProducts
        .Include(cp => cp.DesignElements)  // Lấy danh sách các DesignElement liên quan
        .Include(cp => cp.Product)         // Lấy thông tin sản phẩm
        .Include(cp => cp.User)            // Lấy thông tin người dùng
        .Include(cp => cp.Orders)          // Lấy danh sách các Order liên quan
        .ToListAsync();
        }


        public async Task<List<ProductCustomizationCountDto>> GetProductCustomizationCounts()
        {
            var result = await _context.CustomizeProducts
                .GroupBy(cp => new { cp.ProductId, cp.Product.ProductName, cp.Product.IsDeleted }) // Include IsDeleted in grouping
                .Select(g => new ProductCustomizationCountDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName!,
                    CustomizationCount = g.Count(),
                    IsDeleted = g.Key.IsDeleted // Include IsDeleted in the DTO
                })
                .OrderBy(p => p.IsDeleted) // Non-deleted first (0 first)
                .ThenByDescending(p => p.CustomizationCount) // Sort by customization count within each group
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<CustomizeProduct>> GetByUserIdAsync(int userId)
        {
            return await _context.CustomizeProducts
                 .Include(cp => cp.Product)
                 .Include(cp => cp.DesignElements)
                 .Where(cp => cp.UserId == userId)
                 .ToListAsync();
        }

        public async Task<IEnumerable<CustomizeProduct>> GetByProductIdAsync(int productId)
        {
            return await _context.CustomizeProducts
               .Include(cp => cp.User)
               .Include(cp => cp.DesignElements)
               .Where(cp => cp.ProductId == productId)
               .ToListAsync();
        }

        public async Task<CustomizeProduct> GetWithElementsAsync(int id)
        {
            return await _context.CustomizeProducts
                .Include(cp => cp.DesignElements)
                .Include(cp => cp.Product)
                .Include(cp => cp.User)
                .FirstOrDefaultAsync(cp => cp.CustomizeProductId == id);
        }

        public async Task<IEnumerable<CustomizeProduct>> GetAllWithProductAndUserAsync()
        {
            return await _context.CustomizeProducts
                .Include(cp => cp.Product)
                .Include(cp => cp.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomizeProduct>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.CustomizeProducts
                .Include(cp => cp.DesignElements)
                .Include(cp => cp.Product)
                .Include(cp => cp.User)
                .Include(cp => cp.Orders)
                .Skip((pageNumber - 1) * pageSize) // Bỏ qua số lượng sản phẩm không cần
                .Take(pageSize) // Giới hạn số lượng sản phẩm trong trang
                .ToListAsync();
        }

    }
}
