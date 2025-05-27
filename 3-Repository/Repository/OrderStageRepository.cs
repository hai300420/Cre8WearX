using BusinessObject.Model;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class OrderStageRepository : GenericRepository<OrderStage>, IOrderStageRepository
    {
        private readonly ClothesCusShopContext _context;

        public OrderStageRepository(ClothesCusShopContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<OrderStage>> GetAllOrderStagesAsync()
        {

            return await _context.OrderStages.ToListAsync();



        }

        public async Task<OrderStage?> GetOrderStageByIdAsync(int id)
        {

            return await _context.OrderStages.FirstOrDefaultAsync(o => o.OrderStageId == id);
        }

        public async Task AddOrderStageAsync(OrderStage orderStage)
        {
            //await _context.OrderStages.AddAsync(orderStage);
            await _context.OrderStages.AddAsync(orderStage);
            await _context.SaveChangesAsync(); // Ensure changes are committed
        }

        public async Task UpdateOrderStageAsync(OrderStage orderStage)
        {
            _context.OrderStages.Update(orderStage);
            await _context.SaveChangesAsync(); // Ensure the changes are committed
        }


        public async Task DeleteOrderStageAsync(OrderStage orderStage)
        {
            _context.OrderStages.Remove(orderStage);
        }


        // 🔹 New method to get OrderStage by OrderId
        public async Task<OrderStage?> GetOrderStageByOrderIdAsync(int orderId)
        {
            return await _context.OrderStages
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<OrderStage?> GetLatestOrderStageByOrderIdAsync(int orderId)
        {
            return await _context.OrderStages
                .Where(os => os.OrderId == orderId)
                .OrderByDescending(os => os.UpdatedDate)
                .FirstOrDefaultAsync(); // Only return the latest order stage
        }

    }
}
