using BusinessObject.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IOrderStageRepository : IGenericRepository<OrderStage>
    {
        Task<IEnumerable<OrderStage>> GetAllOrderStagesAsync();
        Task<OrderStage?> GetOrderStageByIdAsync(int id);
        Task AddOrderStageAsync(OrderStage orderStage);
        Task UpdateOrderStageAsync(OrderStage orderStage);
        Task DeleteOrderStageAsync(OrderStage orderStage);
        Task<OrderStage?> GetOrderStageByOrderIdAsync(int orderId);
        Task<OrderStage?> GetLatestOrderStageByOrderIdAsync(int orderId);
    }
}
