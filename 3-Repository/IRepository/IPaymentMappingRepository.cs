using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _3_Repository.IRepository
{
    public interface IPaymentMappingRepository
    {
        Task SaveMappingAsync(PaymentMapping mapping);
        Task<PaymentMapping?> GetByVnpTxnRefAsync(string vnpTxnRef);
    }

}
