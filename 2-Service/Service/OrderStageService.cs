using _2_Service.Service.IService;
using AutoMapper;
using BusinessObject.Enum;
using BusinessObject.Model;
using BusinessObject.ResponseDTO;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.ResponseDTO.ResponseDTO;

namespace Service.Service
{
    //public interface IOrderStageService
    //{
    //    Task<ResponseDTO> GetAllOrderStagesAsync();
    //    Task<ResponseDTO> GetOrderStageByIdAsync(int id);
    //    Task<ResponseDTO> CreateOrderStageAsync(OrderStageCreateDTO orderStageDto);
    //    Task<ResponseDTO> DeleteOrderStageAsync(int id);

    //    //Task CreateOrderStageAsync(OrderStage orderStageDto);

    //    // new method
    //    Task<ResponseDTO> GetOrderStageByOrderIdAsync(int orderId);
    //    Task<ResponseDTO> UpdateOrderStageAsync(OrderStage existingOrderStage);
    //    Task<OrderStage?> GetLatestStageByOrderIdAsync(int orderId);

    //}


    public class OrderStageService : IOrderStageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOrderStageRepository _orderStageRepository;


        public OrderStageService(IUnitOfWork unitOfWork, IMapper mapper, IOrderStageRepository orderStageRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _orderStageRepository = orderStageRepository;
        }

        public async Task<ResponseDTO> GetAllOrderStagesAsync()
        {
            var orderStages = await _unitOfWork.OrderStageRepository.GetAllOrderStagesAsync();
            var result = _mapper.Map<List<ResponseDTO.OrderStageListDTO>>(orderStages);
            return new ResponseDTO(200, "Success", result);
        }

        public async Task<ResponseDTO> GetOrderStageByIdAsync(int id)
        {
            var orderStage = await _unitOfWork.OrderStageRepository.GetOrderStageByIdAsync(id);
            if (orderStage == null) return new ResponseDTO(404, "OrderStage not found");

            var result = new OrderStageResponseDTO
            {
                OrderStageId = orderStage.OrderStageId,
                OrderId = orderStage.OrderId,
                OrderStageName = orderStage.OrderStageName.ToString(),
                UpdatedDate = orderStage.UpdatedDate
            };

            return new ResponseDTO(200, "Success", result);
        }


        public async Task<ResponseDTO> CreateOrderStageAsync(OrderStageCreateDTO orderStageDto)
        {
            // Kiểm tra OrderId có tồn tại không
            var existingOrder = await _unitOfWork.OrderRepository.GetByIdAsync(orderStageDto.OrderId);
            if (existingOrder == null)
            {
                return new ResponseDTO(400, $"OrderId {orderStageDto.OrderId} does not exist.");
            }

            // Kiểm tra giá trị hợp lệ của OrderStageName
            if (string.IsNullOrWhiteSpace(orderStageDto.OrderStageName))
            {
                return new ResponseDTO(400, "OrderStageName cannot be empty.");
            }

            // Prevent duplicate stage for the same order
            var existingOrderStage = (await _unitOfWork.OrderStageRepository.GetAllOrderStagesAsync())
                             .FirstOrDefault(s => s.OrderId == orderStageDto.OrderId &&
                             s.OrderStageName.Trim().ToLower() == orderStageDto.OrderStageName.Trim().ToLower());

            if (existingOrderStage != null)
            {
                // Update instead of adding
                existingOrderStage.UpdatedDate = DateTime.Now;
                await _unitOfWork.OrderStageRepository.UpdateOrderStageAsync(existingOrderStage);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDTO(200, $"OrderStage '{orderStageDto.OrderStageName}' updated successfully", existingOrderStage);
            }

            //if (existingStage != null)
            //{
            //    return new ResponseDTO(409, $"OrderStage '{orderStageDto.OrderStageName}' already exists for OrderId {orderStageDto.OrderId}.");
            //}

            // Map and save OrderStage
            var orderStage = _mapper.Map<OrderStage>(orderStageDto);
            await _unitOfWork.OrderStageRepository.AddOrderStageAsync(orderStage);
            await _unitOfWork.SaveChangesAsync();

            // Create payment if "Hoan thanh"
            if (orderStageDto.OrderStageName.Trim() == "Hoàn thành")
            {
                await CreatePaymentForOrderAsync(existingOrder.OrderId, existingOrder.TotalPrice);
            }

            return new ResponseDTO(201, "OrderStage created successfully", orderStage);
        }


        public async Task<ResponseDTO> DeleteOrderStageAsync(int id)
        {
            var orderStage = await _unitOfWork.OrderStageRepository.GetOrderStageByIdAsync(id);
            if (orderStage == null)
            {
                return new ResponseDTO(404, $"OrderStage with ID {id} not found.");
            }

            await _unitOfWork.OrderStageRepository.DeleteOrderStageAsync(orderStage);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseDTO(200, "OrderStage deleted successfully.");
        }

        // New method to update an existing OrderStage
        public async Task<ResponseDTO> UpdateOrderStageAsync(OrderStage existingOrderStage)
        {
            var orderStage = await _unitOfWork.OrderStageRepository.GetLatestOrderStageByOrderIdAsync(existingOrderStage.OrderId);
            if (orderStage == null)
            {
                return new ResponseDTO(404, $"OrderStage with ID {existingOrderStage.OrderStageId} not found.");
            }

            // Update values
            orderStage.OrderStageName = existingOrderStage.OrderStageName;
            orderStage.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.OrderStageRepository.UpdateOrderStageAsync(orderStage);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseDTO(200, "OrderStage updated successfully.", orderStage);
        }

        public async Task<ResponseDTO> GetOrderStageByOrderIdAsync(int orderId)
        {
            var orderStage = await _unitOfWork.OrderStageRepository.GetOrderStageByOrderIdAsync(orderId);
            if (orderStage == null)
            {
                return new ResponseDTO(404, $"OrderStage for OrderId {orderId} not found.");
            }

            return new ResponseDTO(200, "Success", orderStage);
        }

        public async Task<OrderStage?> GetLatestStageByOrderIdAsync(int orderId)
        {
            return await _unitOfWork.OrderStageRepository.GetAll()
                .Where(s => s.OrderId == orderId)
                .OrderByDescending(s => s.OrderStageId)
                .FirstOrDefaultAsync();
        }


        private async Task CreatePaymentForOrderAsync(int orderId, decimal? totalAmount)
        {
            // Check if a payment already exists for this order
            var existingPayment = await _unitOfWork.PaymentRepository.GetPaymentByOrderIdAsync(orderId);

            // Prevent duplication
            if (existingPayment != null)
            {
                return;
            }

            var payment = new Payment
            {
                OrderId = orderId,
                TotalAmount = totalAmount,
                DepositAmount = totalAmount,
                DepositPaid = 0.01m,
                PaymentDate = DateTime.UtcNow
            };

            await _unitOfWork.PaymentRepository.SavePaymentAsync(payment);
            await _unitOfWork.SaveChangesAsync();
        }


    }
}
