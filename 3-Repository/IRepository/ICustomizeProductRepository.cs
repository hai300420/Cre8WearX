using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Model;
using Repository;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _3_Repository.IRepository
{
    public interface ICustomizeProductRepository : IGenericRepository<CustomizeProduct>
    {
        Task<IEnumerable<CustomizeProduct>> GetAllAsync();
        Task<CustomizeProduct> GetByIdAsync(int id);
        Task AddAsync(CustomizeProduct customizeProduct);
        Task UpdateAsync(CustomizeProduct customizeProduct);
        Task DeleteAsync(int id);

        Task<List<ProductCustomizationCountDto>> GetProductCustomizationCounts();

        Task<IEnumerable<CustomizeProduct>> GetByUserIdAsync(int userId);
        Task<IEnumerable<CustomizeProduct>> GetByProductIdAsync(int productId);
        Task<CustomizeProduct> GetWithElementsAsync(int id);
        Task<IEnumerable<CustomizeProduct>> GetAllWithProductAndUserAsync();
        Task<IEnumerable<CustomizeProduct>> GetAllAsync(int pageNumber, int pageSize);
        Task<IEnumerable<CustomizeProduct>> GetAllAsync(int pageNumber, int pageSize, int userId);

    }
}
