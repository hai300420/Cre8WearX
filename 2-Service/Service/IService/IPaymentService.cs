using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_Service.Service.IService
{
    public interface IPaymentService
    {
        Task SavePaymentAsync(Payment payment);
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
    }
}
