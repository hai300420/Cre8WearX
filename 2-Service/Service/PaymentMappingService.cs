using _2_Service.Service.IService;
using _3_Repository.IRepository;
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
    //#region Old code
    //public interface IPaymentMappingService
    //{
    //    Task SaveMappingAsync(PaymentMapping mapping);
    //    Task<PaymentMapping?> GetByVnpTxnRefAsync(string vnpTxnRef);
    //}
    //public class PaymentMappingService : IPaymentMappingService
    //{
    //    private readonly ClothesCusShopContext _context;

    //    public PaymentMappingService(ClothesCusShopContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task SaveMappingAsync(PaymentMapping mapping)
    //    {
    //        await _context.PaymentMappings.AddAsync(mapping);
    //        await _context.SaveChangesAsync();
    //    }

    //    public async Task<PaymentMapping?> GetByVnpTxnRefAsync(string vnpTxnRef)
    //    {
    //        return await _context.PaymentMappings.FirstOrDefaultAsync(m => m.VnpTxnRef == vnpTxnRef);
    //    }
    //}
    //#endregion

    public class PaymentMappingService : IPaymentMappingService
    {
        private readonly IPaymentMappingRepository _paymentMappingRepository;

        public PaymentMappingService(IPaymentMappingRepository paymentMappingRepository)
        {
            _paymentMappingRepository = paymentMappingRepository;
        }

        public Task SaveMappingAsync(PaymentMapping mapping)
        {
            return _paymentMappingRepository.SaveMappingAsync(mapping);
        }

        public Task<PaymentMapping?> GetByVnpTxnRefAsync(string vnpTxnRef)
        {
            return _paymentMappingRepository.GetByVnpTxnRefAsync(vnpTxnRef);
        }
    }


}
