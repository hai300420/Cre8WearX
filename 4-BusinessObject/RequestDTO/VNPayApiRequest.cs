using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4_BusinessObject.RequestDTO
{
    public class VNPayApiRequest
    {
        public int acqId { get; set; }
        public long accountNo { get; set; }
        public string accountName { get; set; }
        public int amount { get; set; }
        public string addInfo { get; set; }
        public string format { get; set; } = "text";
        public string template { get; set; } = "compact";
    }
}
