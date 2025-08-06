using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _2_Service.Service.IService
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllMyOrdersAsync();
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id);

        Task<bool> CheckCustomizeProductExists(int customizeProductId);
        Task<decimal> CalculateRevenueAsync(int? day, int? month, int? year);
        Task<List<ProductOrderQuantityDto>> GetOrderedProductQuantities();
        Task<RevenueDto> GetMonthlyRevenueAsync(int year);

        Task<bool> MarkOrderAsPaidAsync(int orderId);
    }


}
