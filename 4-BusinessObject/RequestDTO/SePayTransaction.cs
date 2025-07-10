using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4_BusinessObject.RequestDTO
{
    public class SePayTransaction
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string Phone { get; set; }
        public string BankCode { get; set; }
        public string Message { get; set; }
        public DateTime TransactionDate { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
