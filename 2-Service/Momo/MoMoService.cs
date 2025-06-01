using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace _2_Service.Momo
{
    public class MoMoService : IMoMoService
    {
        private readonly IConfiguration _config;

        public MoMoService(IConfiguration config)
        {
            _config = config;
        }

        public bool ValidateSignature(IQueryCollection query, string receivedSignature)
        {
            string secretKey = _config["MoMo:SecretKey"]; // Stored in appsettings.json

            string rawHash = $"accessKey={query["accessKey"]}" +
                             $"&amount={query["amount"]}" +
                             $"&extraData={query["extraData"]}" +
                             $"&message={query["message"]}" +
                             $"&orderId={query["orderId"]}" +
                             $"&orderInfo={query["orderInfo"]}" +
                             $"&orderType={query["orderType"]}" +
                             $"&partnerCode={query["partnerCode"]}" +
                             $"&payType={query["payType"]}" +
                             $"&requestId={query["requestId"]}" +
                             $"&responseTime={query["responseTime"]}" +
                             $"&resultCode={query["resultCode"]}" +
                             $"&transId={query["transId"]}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
            var computedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return computedSignature == receivedSignature;
        }
    }

}
