using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using _2_Service.Momo;
using _2_Service.Service;
using _2_Service.ThirdPartyService;
using _2_Service.Utils;
using _2_Service.Vnpay;
using _4_BusinessObject.RequestDTO;
using _4_BusinessObject.ResponseDTO;
using _4_BusinessObject.VnPay;
using AutoMapper.Internal;
using BusinessObject.Model;
using BusinessObject.ResponseDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.Service;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.ResponseDTO.ResponseDTO;

namespace _1_SPR25_SWD392_ClothingCustomization.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;
        private readonly IOrderService _orderService;
        private readonly IOrderStageService _orderStageService;
        private readonly IPaymentService _paymentService;
        private readonly TimeZoneInfo vietnamTimeZone;
        private readonly IVnPayService _vnPayService;
        private readonly MoMoConfig _momoConfig;
        private readonly IMoMoService _momoService;
        private readonly ILogger<PaymentController> _logger;


        public PaymentController(IVnpay vnPayservice, IConfiguration configuration, IOrderStageService orderStageService, IOrderService orderService,
            IPaymentService paymentService, IOptions<MoMoConfig> config, IMoMoService momoService, ILogger<PaymentController> logger)
        {
            _vnpay = vnPayservice;
            _configuration = configuration;
            _orderService = orderService;
            _orderStageService = orderStageService;
            _paymentService = paymentService;
            _momoConfig = config.Value;
            _momoService = momoService;
            vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");

            _vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:CallbackUrl"]);
            _momoService = momoService;

            _logger = logger;

        }


        #region Old payment

        /// <summary>
        /// Thực hiện hành động sau khi thanh toán. URL này cần được khai báo với VNPAY để API này hoạt đồng (ví dụ: http://localhost:1234/api/Vnpay/IpnAction)
        /// </summary>
        /// <returns></returns>
        [HttpGet("VNPay/IpnAction")]
        public IActionResult IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                        return Ok();
                    }

                    // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }


        [HttpGet("VNPay/CreatePaymentUrl_Card")]
        public async Task<ActionResult<string>> CreatePaymentUrl(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return BadRequest("Order not found");
                }

                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);
                DateTime utcNow = DateTime.UtcNow; // Lấy thời gian hiện tại theo UTC
                DateTime expireTime = utcNow.AddMinutes(15); // Đặt thời gian hết hạn giao dịch (15 phút sau)

                var request = new VNPAY.NET.Models.PaymentRequest
                {
                    PaymentId = orderId,
                    Money = (double)order.TotalPrice,
                    Description = $"Thanh toán đơn hàng #{orderId}",
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = utcNow, // Thời gian tạo giao dịch UTC
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);
                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("VNPay/Callback")]
        public async Task<IActionResult> Callback()
        {
            try
            {
                var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                if (!paymentResult.IsSuccess)
                {
                    // Chuyển hướng đến trang thất bại nếu thanh toán không thành công
                    return Redirect("https://cre8wrearx.vercel.app/payment-failed");
                }

                // Extract orderId từ vnp_TxnRef
                if (!int.TryParse(Request.Query["vnp_TxnRef"], out int orderId))
                {
                    return BadRequest("Invalid Order ID");
                }

                // Xử lý lưu thông tin thanh toán và cập nhật đơn hàng (giữ nguyên phần này)
                var paymentDto = new PaymentAPIVNP
                {
                    OrderId = orderId,
                    Amount = decimal.Parse(Request.Query["vnp_Amount"]) / 100,
                    BankCode = Request.Query["vnp_BankCode"],
                    BankTranNo = Request.Query["vnp_BankTranNo"],
                    CardType = Request.Query["vnp_CardType"],
                    OrderInfo = Request.Query["vnp_OrderInfo"],
                    PayDate = Request.Query["vnp_PayDate"],
                    ResponseCode = Request.Query["vnp_ResponseCode"],
                    TransactionNo = Request.Query["vnp_TransactionNo"],
                    TransactionStatus = Request.Query["vnp_TransactionStatus"],
                    TxnRef = Request.Query["vnp_TxnRef"],
                    SecureHash = Request.Query["vnp_SecureHash"],
                    CreatedAt = DateTime.UtcNow
                };

                // Lưu thông tin thanh toán và cập nhật đơn hàng (giữ nguyên phần này)
                var paymentEntity = new Payment
                {
                    OrderId = paymentDto.OrderId,
                    TotalAmount = paymentDto.Amount,
                    DepositPaid = paymentDto.Amount,
                    DepositAmount = paymentDto.Amount,
                    PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
                };

                await _paymentService.SavePaymentAsync(paymentEntity);

                // Cập nhật trạng thái đơn hàng (giữ nguyên phần này)
                var existingOrder = await _orderService.GetOrderByIdAsync(paymentDto.OrderId);
                if (existingOrder == null)
                {
                    return BadRequest($"OrderId {paymentDto.OrderId} does not exist.");
                }

                // Xử lý OrderStage (giữ nguyên phần này)
                var response = await _orderStageService.GetOrderStageByOrderIdAsync(paymentDto.OrderId);
                OrderStageResponseDTO? existingOrderStageDto = null;

                if (response.Status == 200 && response.Data is OrderStageResponseDTO orderStageData)
                {
                    existingOrderStageDto = orderStageData;
                }

                if (existingOrderStageDto != null)
                {
                    var existingOrderStage = new OrderStage
                    {
                        OrderStageId = existingOrderStageDto.OrderStageId,
                        OrderId = existingOrderStageDto.OrderId,
                        OrderStageName = "Purchased",
                        UpdatedDate = DateTime.UtcNow
                    };

                    var updateResponse = await _orderStageService.UpdateOrderStageAsync(existingOrderStage);
                    if (updateResponse.Status != 200)
                    {
                        return BadRequest(updateResponse);
                    }
                }
                else
                {
                    var orderStageDto = new OrderStageCreateDTO
                    {
                        OrderId = paymentDto.OrderId,
                        OrderStageName = "Purchased",
                        UpdatedDate = DateTime.UtcNow
                    };

                    var createResponse = await _orderStageService.CreateOrderStageAsync(orderStageDto);
                    if (createResponse.Status != 201)
                    {
                        return BadRequest(response);
                    }
                }

                // Chuyển hướng đến trang thành công
                return Redirect("https://cre8wrearx.vercel.app/payment-success");
            }
            catch (Exception ex)
            {
                // Chuyển hướng đến trang lỗi nếu có exception
                return Redirect("https://cre8wrearx.vercel.app/payment-error?message=" + WebUtility.UrlEncode(ex.Message));
            }
        }

        [HttpPost("VNPay/GenerateQR_Local")]
        public async Task<IActionResult> GenerateQr([FromBody] VNPayApiRequest apiRequest)
        {
            using var httpClient = new HttpClient();

            var jsonRequest = JsonConvert.SerializeObject(apiRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://api.vietqr.io/v2/generate", content);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error from VietQR API");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<VNPayApiResponse>(responseString);
            var base64Image = apiResponse?.data?.qrDataURL?.Replace("data:image/png;base64,", "");

            if (base64Image == null)
                return BadRequest("Failed to parse QR code.");

            //// Convert base64 to byte array
            //byte[] imageBytes = Convert.FromBase64String(base64Image);

            //// Define the path where the image will be saved
            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "qr_code.png");

            //// Save the image to a file
            //await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
            //return Ok(new
            //{
            //    base64 = base64Image,
            //    fullData = apiResponse,
            //    imageUrl = "/images/qr_code.png" // Return the image URL
            //});

            // Save image using the helper class
            string fileName = $"qr_code_{DateTime.UtcNow.Ticks}.png"; // Unique file name
            string imageUrl = await QrCodeSaver.SaveQrImageAsync(base64Image, fileName);

            return Ok(new
            {
                // base64 = base64Image,
                fullData = apiResponse,
                imageUrl = imageUrl
            });

        }

        [HttpPost("VNPay/GenerateQR_Online")]
        public async Task<IActionResult> GenerateQr_Online([FromBody] VNPayApiRequest apiRequest, [FromServices] CloudinaryService cloudinaryService)
        {
            using var httpClient = new HttpClient();

            var jsonRequest = JsonConvert.SerializeObject(apiRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://api.vietqr.io/v2/generate", content);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error from VietQR API");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<VNPayApiResponse>(responseString);
            var base64Image = apiResponse?.data?.qrDataURL?.Replace("data:image/png;base64,", "");

            if (string.IsNullOrWhiteSpace(base64Image))
                return BadRequest("Failed to parse QR code.");

            string fileName = $"qr_code_{DateTime.UtcNow.Ticks}.png";

            // Upload to Cloudinary
            // string imageUrl = await cloudinaryService.UploadBase64ImageAsync(base64Image, fileName);

            //return Ok(new
            //{
            //    // base64 = base64Image,
            //    fullData = apiResponse,
            //    imageUrl = imageUrl
            //});

            var uploadResult = await cloudinaryService.UploadBase64ImageAsync(base64Image, fileName);

            return Ok(new
            {
                // base64 = base64Image,
                fullData = apiResponse,
                imageUrl = uploadResult.Url,

            });
        }


        [HttpGet("MoMo/CreatePaymentUrl_QR")]
        public async Task<ActionResult<string>> CreateMoMoPaymentUrl(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return BadRequest("Order not found");

            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string requestId = Guid.NewGuid().ToString();
            string orderIdStr = orderId.ToString();
            string orderInfo = $"Thanh toán đơn hàng #{orderId}";
            string amount = ((int)order.TotalPrice).ToString();
            string requestType = "captureWallet";
            string extraData = "";

            string rawHash = $"accessKey={_momoConfig.AccessKey}&amount={amount}&extraData={extraData}&ipnUrl={_momoConfig.NotifyUrl}&orderId={orderIdStr}&orderInfo={orderInfo}&partnerCode={_momoConfig.PartnerCode}&redirectUrl={_momoConfig.ReturnUrl}&requestId={requestId}&requestType={requestType}";
            string signature = MoMoSecurity.SignSHA256(rawHash, _momoConfig.SecretKey);

            var requestBody = new
            {
                partnerCode = _momoConfig.PartnerCode,
                partnerName = "MoMoTest",
                storeId = "SwdStore",
                requestId = requestId,
                amount = amount,
                orderId = orderIdStr,
                orderInfo = orderInfo,
                redirectUrl = _momoConfig.ReturnUrl,
                ipnUrl = _momoConfig.NotifyUrl,
                lang = "vi",
                extraData = extraData,
                requestType = requestType,
                signature = signature
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(responseJson);
            var payUrl = json["payUrl"]?.ToString();

            if (string.IsNullOrEmpty(payUrl))
                return BadRequest("Failed to generate payment URL");

            return Created(payUrl, payUrl);
        }


        #region MoMo

        //[HttpGet("MoMo/Callback")]
        //public async Task<IActionResult> Callback([FromBody] MoMoCallbackModel callback)
        //{
        //    try
        //    {
        //        if (callback.ResultCode != 0)
        //            return Redirect("https://cre8wrearx.vercel.app/payment-failed");

        //        if (!int.TryParse(callback.OrderId, out int orderId))
        //            return BadRequest("Invalid Order ID");

        //        var payment = new Payment
        //        {
        //            OrderId = orderId,
        //            TotalAmount = callback.Amount,
        //            DepositPaid = callback.Amount,
        //            DepositAmount = callback.Amount,
        //            PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
        //        };

        //        await _paymentService.SavePaymentAsync(payment);

        //        var existingOrder = await _orderService.GetOrderByIdAsync(orderId);
        //        if (existingOrder == null)
        //            return BadRequest($"OrderId {orderId} does not exist.");

        //        var response = await _orderStageService.GetOrderStageByOrderIdAsync(orderId);
        //        OrderStageResponseDTO? existingOrderStageDto = null;

        //        if (response.Status == 200 && response.Data is OrderStageResponseDTO stageData)
        //            existingOrderStageDto = stageData;

        //        if (existingOrderStageDto != null)
        //        {
        //            var existingOrderStage = new OrderStage
        //            {
        //                OrderStageId = existingOrderStageDto.OrderStageId,
        //                OrderId = existingOrderStageDto.OrderId,
        //                OrderStageName = "Purchased",
        //                UpdatedDate = DateTime.UtcNow
        //            };

        //            var updateResponse = await _orderStageService.UpdateOrderStageAsync(existingOrderStage);
        //            if (updateResponse.Status != 200)
        //                return BadRequest(updateResponse);
        //        }
        //        else
        //        {
        //            var orderStageDto = new OrderStageCreateDTO
        //            {
        //                OrderId = orderId,
        //                OrderStageName = "Purchased",
        //                UpdatedDate = DateTime.UtcNow
        //            };

        //            var createResponse = await _orderStageService.CreateOrderStageAsync(orderStageDto);
        //            if (createResponse.Status != 201)
        //                return BadRequest(response);
        //        }

        //        return Ok(new { message = "MoMo payment processed successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Redirect("https://cre8wrearx.vercel.app/payment-error?message=" + WebUtility.UrlEncode(ex.Message));
        //    }
        //}

        [HttpGet("MoMo/CreatePaymentUrl_Card")]
        public async Task<ActionResult<string>> CreateMoMoBankTransferPaymentUrl(int request)
        {
            var order = await _orderService.GetOrderByIdAsync(request);
            if (order == null)
                return BadRequest("Order not found");

            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string requestId = Guid.NewGuid().ToString();
            string orderIdStr = request.ToString();
            string orderInfo = $"Thanh toán đơn hàng #{request}";
            string amount = ((int)order.TotalPrice).ToString();
            string requestType = "payWithATM";
            string extraData = ""; // optional info you may want to store

            string rawHash = $"accessKey={_momoConfig.AccessKey}&amount={amount}&extraData={extraData}&ipnUrl={_momoConfig.NotifyUrl}&orderId={orderIdStr}&orderInfo={orderInfo}" +
                $"&partnerCode={_momoConfig.PartnerCode}&redirectUrl={_momoConfig.ReturnUrl}&requestId={requestId}&requestType={requestType}";
            string signature = MoMoSecurity.SignSHA256(rawHash, _momoConfig.SecretKey);

            var requestBody = new
            {
                partnerCode = _momoConfig.PartnerCode,
                partnerName = "YourStore",
                storeId = "YourStoreId",
                requestId = requestId,
                amount = amount,
                orderId = orderIdStr,
                orderInfo = orderInfo,
                redirectUrl = _momoConfig.ReturnUrl,
                ipnUrl = _momoConfig.NotifyUrl,
                lang = "vi",
                extraData = extraData,
                requestType = requestType,
                signature = signature
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(responseJson);
            var payUrl = json["payUrl"]?.ToString();
            var resultCode = json["resultCode"]?.ToObject<int>() ?? -1;

            if (resultCode != 0 || string.IsNullOrEmpty(payUrl))
            {
                var message = json["message"]?.ToString() ?? "Unknown error";
                return BadRequest($"MoMo error: {message}");
            }

            return Created(payUrl, payUrl);
        }

        [HttpPost("MoMo/IPN")]
        public async Task<IActionResult> MoMoIpn([FromBody] MoMoIpnDto ipnData)
        {
            // Step 1: Verify signature
            var rawHash = $"accessKey={_momoConfig.AccessKey}" +
                          $"&amount={ipnData.amount}" +
                          $"&extraData={ipnData.extraData}" +
                          $"&message={ipnData.message}" +
                          $"&orderId={ipnData.orderId}" +
                          $"&orderInfo={ipnData.orderInfo}" +
                          $"&orderType={ipnData.orderType}" +
                          $"&partnerCode={ipnData.partnerCode}" +
                          $"&payType={ipnData.payType}" +
                          $"&requestId={ipnData.requestId}" +
                          $"&responseTime={ipnData.responseTime}" +
                          $"&resultCode={ipnData.resultCode}" +
                          $"&transId={ipnData.transId}";

            string calculatedSignature = MoMoSecurity.SignSHA256(rawHash, _momoConfig.SecretKey);
            if (calculatedSignature != ipnData.signature)
                return BadRequest("Invalid signature");

            if (ipnData.resultCode == 0)
            {
                int orderId = int.Parse(ipnData.orderId);

                var payment = new Payment
                {
                    OrderId = orderId,
                    TotalAmount = ipnData.amount,
                    DepositPaid = ipnData.amount,
                    DepositAmount = ipnData.amount,
                    PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
                };

                await _paymentService.SavePaymentAsync(payment);

                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                    return BadRequest("Order not found");

                var response = await _orderStageService.GetOrderStageByOrderIdAsync(orderId);
                if (response.Status == 200 && response.Data is OrderStageResponseDTO stageDto)
                {
                    var updatedStage = new OrderStage
                    {
                        OrderStageId = stageDto.OrderStageId,
                        OrderId = stageDto.OrderId,
                        OrderStageName = "Purchased",
                        UpdatedDate = DateTime.UtcNow
                    };
                    await _orderStageService.UpdateOrderStageAsync(updatedStage);
                }
                else
                {
                    var newStage = new OrderStageCreateDTO
                    {
                        OrderId = orderId,
                        OrderStageName = "Purchased",
                        UpdatedDate = DateTime.UtcNow
                    };
                    await _orderStageService.CreateOrderStageAsync(newStage);
                }
            }

            return Ok(); // Must return 200 OK so MoMo stops retrying
        }



        [HttpGet("MoMo/Return")]
        public IActionResult MoMoReturn([FromQuery] MoMoReturnDto result)
        {
            if (result.resultCode == 0)
            {
                return Redirect("https://cre8wrearx.vercel.app/payment-success");
            }

            return Redirect("https://cre8wrearx.vercel.app/payment-failed");
        }


        //[HttpGet("MoMo/CreatePaymentUrl2")]
        //public async Task<ActionResult<string>> CreateMoMoPaymentUrl2(int orderId)
        //{
        //    try
        //    {
        //        var order = await _orderService.GetOrderByIdAsync(orderId);
        //        if (order == null)
        //            return BadRequest("Order not found");

        //        var config = _configuration.GetSection("MoMo");

        //        string endpoint = config["Endpoint"]; // Example: https://test-payment.momo.vn/v2/gateway/api/create
        //        string partnerCode = config["PartnerCode"];
        //        string accessKey = config["AccessKey"];
        //        string secretKey = config["SecretKey"];
        //        string returnUrl = config["ReturnUrl"];
        //        string notifyUrl = config["NotifyUrl"];

        //        string requestId = Guid.NewGuid().ToString();
        //        string orderInfo = $"Thanh toán đơn hàng #{orderId}";
        //        string amount = ((long)order.TotalPrice).ToString(); // must be string in VND, not decimal
        //        string orderIdStr = orderId.ToString();
        //        string extraData = ""; // optional, can be base64 encoded user info or leave empty

        //        // Build raw signature string
        //        string rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={notifyUrl}" +
        //                 $"&orderId={orderIdStr}&orderInfo={orderInfo}&orderType=momoATM&partnerCode={partnerCode}" +
        //                 $"&redirectUrl={returnUrl}&requestId={requestId}&requestType=payWithATM";

        //        // Generate HMAC SHA256 signature
        //        string signature;
        //        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
        //        {
        //            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
        //            signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        //        }

        //        var requestBody = new
        //        {
        //            partnerCode,
        //            accessKey,
        //            requestId,
        //            amount,
        //            orderId = orderIdStr,
        //            orderInfo,
        //            orderType = "momoATM", // ATM bank payment
        //            redirectUrl = returnUrl,
        //            ipnUrl = notifyUrl,
        //            extraData,
        //            requestType = "payWithATM",
        //            signature,
        //            lang = "vi"
        //        };

        //        using var httpClient = new HttpClient();
        //        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        //        var response = await httpClient.PostAsync(endpoint, content);
        //        var responseString = await response.Content.ReadAsStringAsync();

        //        var jsonDoc = JsonDocument.Parse(responseString);
        //        if (jsonDoc.RootElement.TryGetProperty("payUrl", out var payUrlElement))
        //        {
        //            var payUrl = payUrlElement.GetString();
        //            return Created(payUrl, payUrl);
        //        }

        //        return BadRequest("Failed to get MoMo payment URL");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}



        [HttpGet("MoMo/Callback")]
        public async Task<IActionResult> MoMoCallback()
        {
            try
            {
                var query = Request.Query;
                string signature = query["signature"];

                if (!_momoService.ValidateSignature(query, signature))
                {
                    return Redirect("https://cre8wrearx.vercel.app/payment-failed");
                }

                if (query["resultCode"] != "0")
                {
                    return Redirect("https://cre8wrearx.vercel.app/payment-failed");
                }

                if (!int.TryParse(query["orderId"], out int orderId))
                {
                    return BadRequest("Invalid Order ID");
                }

                var paymentDto = new PaymentAPIMoMo
                {
                    OrderId = orderId,
                    Amount = decimal.Parse(query["amount"]) / 100, // If amount is in smallest currency unit
                    OrderInfo = query["orderInfo"],
                    OrderType = query["orderType"],
                    TransId = query["transId"],
                    ResultCode = query["resultCode"],
                    Message = query["message"],
                    PayType = query["payType"],
                    ResponseTime = query["responseTime"],
                    ExtraData = query["extraData"],
                    Signature = query["signature"],
                    CreatedAt = DateTime.UtcNow
                };

                var paymentEntity = new Payment
                {
                    OrderId = paymentDto.OrderId,
                    TotalAmount = paymentDto.Amount,
                    DepositPaid = paymentDto.Amount,
                    DepositAmount = paymentDto.Amount,
                    PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
                };

                await _paymentService.SavePaymentAsync(paymentEntity);

                var existingOrder = await _orderService.GetOrderByIdAsync(paymentDto.OrderId);
                if (existingOrder == null)
                {
                    return BadRequest($"OrderId {paymentDto.OrderId} does not exist.");
                }

                var response = await _orderStageService.GetOrderStageByOrderIdAsync(paymentDto.OrderId);
                OrderStageResponseDTO? existingOrderStageDto = null;

                if (response.Status == 200 && response.Data is OrderStageResponseDTO orderStageData)
                {
                    existingOrderStageDto = orderStageData;
                }

                if (existingOrderStageDto != null)
                {
                    var existingOrderStage = new OrderStage
                    {
                        OrderStageId = existingOrderStageDto.OrderStageId,
                        OrderId = existingOrderStageDto.OrderId,
                        OrderStageName = "Purchased",
                        UpdatedDate = DateTime.UtcNow
                    };

                    var updateResponse = await _orderStageService.UpdateOrderStageAsync(existingOrderStage);
                    if (updateResponse.Status != 200)
                    {
                        return BadRequest(updateResponse);
                    }
                }
                else
                {
                    var orderStageDto = new OrderStageCreateDTO
                    {
                        OrderId = paymentDto.OrderId,
                        OrderStageName = "Purchased",
                        UpdatedDate = DateTime.UtcNow
                    };

                    var createResponse = await _orderStageService.CreateOrderStageAsync(orderStageDto);
                    if (createResponse.Status != 201)
                    {
                        return BadRequest(response);
                    }
                }

                return Redirect("https://cre8wrearx.vercel.app/payment-success");
            }
            catch (Exception ex)
            {
                return Redirect("https://cre8wrearx.vercel.app/payment-error?message=" + WebUtility.UrlEncode(ex.Message));
            }
        }

        #endregion

        [HttpPost("MoMo/SimulateCallback")]
        public async Task<IActionResult> SimulateMoMoCallback([FromBody] PaymentAPIMoMo paymentDto)
        {
            // Simulate what MoMo IPN callback would do:

            var paymentEntity = new Payment
            {
                OrderId = paymentDto.OrderId,
                TotalAmount = paymentDto.Amount,
                DepositPaid = paymentDto.Amount,
                DepositAmount = paymentDto.Amount,
                PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
            };

            await _paymentService.SavePaymentAsync(paymentEntity);

            var existingOrder = await _orderService.GetOrderByIdAsync(paymentDto.OrderId);
            if (existingOrder == null)
            {
                return BadRequest($"OrderId {paymentDto.OrderId} does not exist.");
            }

            var response = await _orderStageService.GetOrderStageByOrderIdAsync(paymentDto.OrderId);
            OrderStageResponseDTO? existingOrderStageDto = null;

            if (response.Status == 200 && response.Data is OrderStageResponseDTO orderStageData)
            {
                existingOrderStageDto = orderStageData;
            }

            if (existingOrderStageDto != null)
            {
                var existingOrderStage = new OrderStage
                {
                    OrderStageId = existingOrderStageDto.OrderStageId,
                    OrderId = existingOrderStageDto.OrderId,
                    OrderStageName = "Purchased",
                    UpdatedDate = DateTime.UtcNow
                };

                var updateResponse = await _orderStageService.UpdateOrderStageAsync(existingOrderStage);
                if (updateResponse.Status != 200)
                {
                    return BadRequest(updateResponse);
                }
            }
            else
            {
                var orderStageDto = new OrderStageCreateDTO
                {
                    OrderId = paymentDto.OrderId,
                    OrderStageName = "Purchased",
                    UpdatedDate = DateTime.UtcNow
                };

                var createResponse = await _orderStageService.CreateOrderStageAsync(orderStageDto);
                if (createResponse.Status != 201)
                {
                    return BadRequest(response);
                }
            }

            return Ok("Simulated MoMo IPN callback success");
        }
        #endregion

        [HttpPost("SePay/webhook")]
        public async Task<IActionResult> ReceivePayment([FromBody] SepayWebhookRequest request)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var apiKeyHeader))
            {
                return Unauthorized();
            }

            var expectedKey = "Apikey 78f1b3a8-4e21-4c9f-8a8a-2f29fcb45601";
            if (apiKeyHeader != expectedKey)
            {
                return Unauthorized();
            }

            _logger.LogInformation("Webhook authorized. Processing...");

            // Save transaction or trigger logic
            return Ok(new { success = true });

        }

    }
}