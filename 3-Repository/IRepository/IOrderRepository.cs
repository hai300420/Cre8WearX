using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObject.Model;
using BusinessObject.RequestDTO;

namespace Repository.IRepository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<CustomizeProduct?> GetCustomizeProductByIdAsync(int id);
        Task<bool> CheckCustomizeProductExists(int customizeProductId);
        Task<Order> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
        Task<List<RequestDTO.ProductOrderQuantityDto>> GetOrderedProductQuantities();
    }
}
