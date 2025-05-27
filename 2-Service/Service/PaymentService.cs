using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_Service.Service
{
    public interface IPaymentService
    {
        Task SavePaymentAsync(Payment payment);
    }

    public class PaymentService : IPaymentService
    {
        private readonly ClothesCusShopContext _context;

        public PaymentService(ClothesCusShopContext context)
        {
            _context = context;
        }

        public async Task SavePaymentAsync(Payment payment)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var vietnamTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, vietnamTimeZone);

            payment.PaymentDate = vietnamTime.DateTime; // Lấy phần DateTime chuẩn
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }
    }
}
