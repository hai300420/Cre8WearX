using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_Service.ThirdPartyService
{
    public class SePayQRService
    {
        private const string Bank = "TPBank";
        private const string AccountNumber = "95300403741"; // Bank account

        public string GenerateOrderQrBase64(int orderId, decimal amount)
        {
            // Content shown to user and extracted later
            var transactionContent = $"Thanh toan don hang {orderId}";

            // This is only for internal format – you can customize
            string qrData = $"Bank: {Bank}\nAccount: {AccountNumber}\nAmount: {amount}\nContent: {transactionContent}";

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new Base64QRCode(qrCodeData);
            return qrCode.GetGraphic(20); // 20 = pixels per module
        }
    }
}
