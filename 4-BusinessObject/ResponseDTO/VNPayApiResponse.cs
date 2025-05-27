using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4_BusinessObject.ResponseDTO
{
    public class VNPayApiResponse
    {
        public int code { get; set; }
        public string desc { get; set; }
        public Data data { get; set; }
    }
    public class Data
    {
        public string qrDataURL { get; set; }
    }
}
