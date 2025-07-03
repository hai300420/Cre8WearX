﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4_BusinessObject.RequestDTO
{
    public class SepayWebhookRequest
    {
        public int Id { get; set; }
        public string Gateway { get; set; }
        public DateTime TransactionDate { get; set; }
        public string AccountNumber { get; set; }
        public string Code { get; set; }
        public string Content { get; set; }
        public string TransferType { get; set; }
        public decimal TransferAmount { get; set; }
        public decimal Accumulated { get; set; }
        public string SubAccount { get; set; }
        public string ReferenceCode { get; set; }
        public string Description { get; set; }
    }
}
