using _3_Repository.IRepository;
using BusinessObject.Model;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Repository.Repository
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly ClothesCusShopContext _context;

        public PaymentRepository(ClothesCusShopContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            var payments = await _context.Payments
                .ToListAsync();

            return payments;
        }

        public async Task SavePaymentAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }
    }
}
