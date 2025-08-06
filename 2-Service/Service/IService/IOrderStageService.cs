using BusinessObject.Model;
using BusinessObject.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _2_Service.Service.IService
{
    public interface IOrderStageService
    {
        Task<ResponseDTO> GetAllOrderStagesAsync();
        Task<ResponseDTO> GetOrderStageByIdAsync(int id);
        Task<ResponseDTO> CreateOrderStageAsync(OrderStageCreateDTO orderStageDto);
        Task<ResponseDTO> DeleteOrderStageAsync(int id);

        //Task CreateOrderStageAsync(OrderStage orderStageDto);

        // new method
        Task<ResponseDTO> GetOrderStageByOrderIdAsync(int orderId);
        Task<ResponseDTO> UpdateOrderStageAsync(OrderStage existingOrderStage);
        Task<OrderStage?> GetLatestStageByOrderIdAsync(int orderId);

    }
}
