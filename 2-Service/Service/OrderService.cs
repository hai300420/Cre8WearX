using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObject.Model;
using _3_Repository.IRepository;
using Repository.IRepository;
using System;
using Repository.Repository;
using static BusinessObject.RequestDTO.RequestDTO;
using Microsoft.AspNetCore.Http;
using _3_Repository.Repository;
using BusinessObject.ResponseDTO;
using BusinessObject;
using _2_Service.Service.IService;

namespace _2_Service.Service
{
    //public interface IOrderService
    //{
    //    Task<IEnumerable<Order>> GetAllOrdersAsync();
    //    Task<Order> GetOrderByIdAsync(int id);
    //    Task<IEnumerable<Order>> GetAllMyOrdersAsync();
    //    Task AddOrderAsync(Order order);
    //    Task UpdateOrderAsync(Order order);
    //    Task DeleteOrderAsync(int id);
        
    //    Task<bool> CheckCustomizeProductExists(int customizeProductId);
    //    Task<decimal> CalculateRevenueAsync(int? day, int? month, int? year);
    //    Task<List<ProductOrderQuantityDto>> GetOrderedProductQuantities();
    //    Task<RevenueDto> GetMonthlyRevenueAsync(int year);

    //    Task<bool> MarkOrderAsPaidAsync(int orderId);
    //}

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStageRepository _orderStageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(IOrderRepository orderRepository, IOrderStageRepository orderStageRepository, IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _orderStageRepository = orderStageRepository;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            //return await _orderRepository.GetAllAsync();
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetAllMyOrdersAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
                                .FirstOrDefault(c => c.Type == "User_Id");

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return Enumerable.Empty<Order>(); // Return empty
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Enumerable.Empty<Order>(); // Return empty
            }

            return await _orderRepository.GetAllMyOrdersAsync(userId);
        }

        //public async Task AddOrderAsync(Order order)
        //{
        //    if (order.CustomizeProductId <= 0)
        //    {
        //        throw new ArgumentException("CustomizeProductId must be greater than 0.");
        //    }

        //    // Kiểm tra `CustomizeProductId` có tồn tại trong database hay không
        //    var existingProduct = await _orderRepository.GetCustomizeProductByIdAsync(order.CustomizeProductId);
        //    if (existingProduct == null)
        //    {
        //        throw new ArgumentException($"CustomizeProductId {order.CustomizeProductId} does not exist. Please provide a valid CustomizeProductId.");
        //    }

        //    if (order.Price <= 0 || order.Quantity <= 0 || order.TotalPrice <= 0)
        //    {
        //        throw new ArgumentException("Price, Quantity, and TotalPrice must be greater than zero.");
        //    }

        //    await _orderRepository.AddAsync(order);


        //    // Add order and ensure OrderId is generated
        //    await _orderRepository.AddAsync(order); // Save order

        //    // **Reload the order from DB to get the generated OrderId**
        //    var savedOrder = await _orderRepository.GetByIdAsync(order.OrderId);

        //    if (savedOrder == null)
        //    {
        //        throw new Exception("Failed to retrieve saved order.");
        //    }

        //    // Now, add order stage using the saved OrderId
        //    OrderStage orderStage = new OrderStage
        //    {
        //        OrderId = savedOrder.OrderId,
        //        OrderStageName = "Place Order",
        //        UpdatedDate = DateTime.Now
        //    };

        //    await _orderStageRepository.AddOrderStageAsync(orderStage);
        //}

        public async Task AddOrderAsync(Order order)
        {
            try
            {
                if (order.CustomizeProductId <= 0)
                {
                    throw new ArgumentException("CustomizeProductId must be greater than 0.");
                }

                var existingProduct = await _orderRepository.GetCustomizeProductByIdAsync(order.CustomizeProductId);
                if (existingProduct == null)
                {
                    throw new ArgumentException($"CustomizeProductId {order.CustomizeProductId} does not exist. Please provide a valid CustomizeProductId.");
                }

                if (order.Price <= 0 || order.Quantity <= 0)
                {
                    throw new ArgumentException("Price and Quantity must be greater than zero.");
                }


                // Tự động tính TotalPrice
                order.TotalPrice = order.Price * order.Quantity;

                // Add order
                await _orderRepository.AddAsync(order);

                // Fetch the saved order
                var savedOrder = await _orderRepository.GetByIdAsync(order.OrderId);
                if (savedOrder == null)
                {
                    throw new Exception("Failed to retrieve saved order.");
                }

                // Add order stage
                OrderStage orderStage = new OrderStage
                {
                    OrderId = savedOrder.OrderId,
                    // OrderStageName = "Place Order", 
                    OrderStageName = "Đặt hàng",
                    UpdatedDate = DateTime.Now
                };

                await _orderStageRepository.AddOrderStageAsync(orderStage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }



        public async Task UpdateOrderAsync(Order order)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(order.OrderId);
            if (existingOrder == null)
            {
                throw new Exception("Order not found.");
            }

            // Kiểm tra CustomizeProductId có hợp lệ không
            if (order.CustomizeProductId <= 0)
            {
                throw new ArgumentException("CustomizeProductId must be greater than 0.");
            }

            // Kiểm tra các giá trị quan trọng
            if (order.Price <= 0 || order.Quantity <= 0)
            {
                throw new ArgumentException("Price, Quantity, and TotalPrice must be greater than zero.");
            }
            order.TotalPrice = order.Price * order.Quantity;

            await _orderRepository.UpdateAsync(order);
        }
        public async Task<bool> CheckCustomizeProductExists(int customizeProductId)
        {
            return await _orderRepository.CheckCustomizeProductExists(customizeProductId);
        }


        public async Task DeleteOrderAsync(int id)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(id);
            if (existingOrder == null)
            {
                throw new ArgumentException($"Order with ID {id} not found. Cannot delete.");
            }

            await _orderRepository.DeleteAsync(id);
        }


        // view revenue
        public async Task<decimal> CalculateRevenueAsync(int? day, int? month, int? year)
        {
            var orders = await _orderRepository.GetAllAsync();

            var filteredOrders = orders.Where(order =>
                (!day.HasValue || (order.OrderDate.HasValue && order.OrderDate.Value.Day == day)) &&
                (!month.HasValue || (order.OrderDate.HasValue && order.OrderDate.Value.Month == month)) &&
                (!year.HasValue || (order.OrderDate.HasValue && order.OrderDate.Value.Year == year))
            );

            return filteredOrders.Sum(order => order.TotalPrice.GetValueOrDefault());
        }


        public async Task<List<ProductOrderQuantityDto>> GetOrderedProductQuantities()
        {
            var orders = await _orderRepository.GetOrderedProductQuantities();

            // Business Logic: If no orders, return empty list
            if (orders == null || !orders.Any())
            {
                return new List<ProductOrderQuantityDto>();
            }

            return orders;
        }

        public async Task<RevenueDto> GetMonthlyRevenueAsync(int year)
        {
            var orders = await _orderRepository.GetAllOrdersAsync();

            // Group by month and sum TotalPrice
            var monthlyRevenue = orders
                .Where(order => order.OrderDate.HasValue && order.OrderDate.Value.Year == year)
                .GroupBy(order => order.OrderDate.Value.Month)
                .OrderBy(g => g.Key)
                .Select(g => new { Month = g.Key, Revenue = g.Sum(o => o.TotalPrice ?? 0) })
                .ToList();

            // Create a response in the required format
            var revenueDto = new RevenueDto
            {
                //        Labels = new List<string>
                //{
                //    "January", "February", "March", "April", "May", "June",
                //    "July", "August", "September", "October", "November", "December"
                //},
                Labels = new List<string>
                {
                    "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6",
                    "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
                },
                Datasets = new List<RevenueDataset>
        {
            new RevenueDataset
            {
                Data = Enumerable.Range(1, 12)
                    .Select(month => monthlyRevenue.FirstOrDefault(m => m.Month == month)?.Revenue ?? 0)
                    .ToList()
            }
        }
            };

            return revenueDto;
        }

        public async Task<bool> MarkOrderAsPaidAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return false;

            // Add a new OrderStage to mark payment completion
            var paymentStage = new OrderStage
            {
                OrderId = orderId,
                // OrderStageName = "Payment Completed", // or "Đã thanh toán"
                OrderStageName = "Đã thanh toán", // or "Đã thanh toán"
                UpdatedDate = DateTime.Now
            };

            await _orderStageRepository.AddOrderStageAsync(paymentStage);

            return true;
        }



    }
}
