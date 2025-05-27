using BusinessObject.Model;
using Google;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _2_Service.Service
{
    public interface IPaymentMappingService
    {
        Task SaveMappingAsync(PaymentMapping mapping);
        Task<PaymentMapping?> GetByVnpTxnRefAsync(string vnpTxnRef);
    }
    public class PaymentMappingService : IPaymentMappingService
    {
        private readonly ClothesCusShopContext _context;

        public PaymentMappingService(ClothesCusShopContext context)
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
