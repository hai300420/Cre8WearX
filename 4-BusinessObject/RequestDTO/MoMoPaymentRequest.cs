using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4_BusinessObject.RequestDTO
{
    public class MoMoPaymentRequest
    {
        public int Amount { get; set; }
        public string OrderInfo { get; set; }
    }
}
