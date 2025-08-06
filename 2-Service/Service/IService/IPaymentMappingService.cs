using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _2_Service.Service.IService
{
    public interface IPaymentMappingService
    {
        Task SaveMappingAsync(PaymentMapping mapping);
        Task<PaymentMapping?> GetByVnpTxnRefAsync(string vnpTxnRef);
    }


}
