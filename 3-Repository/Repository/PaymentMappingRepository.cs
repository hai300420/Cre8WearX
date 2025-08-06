using _3_Repository.IRepository;
using BusinessObject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _3_Repository.Repository
{
    public class PaymentMappingRepository : IPaymentMappingRepository
    {
        private readonly ClothesCusShopContext _context;

        public PaymentMappingRepository(ClothesCusShopContext context)
        {
            _context = context;
        }

        public async Task SaveMappingAsync(PaymentMapping mapping)
        {
            await _context.PaymentMappings.AddAsync(mapping);
            await _context.SaveChangesAsync();
        }

        public async Task<PaymentMapping?> GetByVnpTxnRefAsync(string vnpTxnRef)
        {
            return await _context.PaymentMappings.FirstOrDefaultAsync(m => m.VnpTxnRef == vnpTxnRef);
        }
    }
}
