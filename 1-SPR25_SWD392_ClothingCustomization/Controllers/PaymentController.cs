using System.Net;
using System.Text;
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

        public PaymentController(IVnpay vnPayservice, IConfiguration configuration, IOrderStageService orderStageService, IOrderService orderService, IPaymentService paymentService, IOptions<MoMoConfig> config)
        {
            _vnpay = vnPayservice;
            _configuration = configuration;
            _orderService = orderService;
            _orderStageService = orderStageService;
            _paymentService = paymentService;
            _momoConfig = config.Value;
            vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");

            _vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:CallbackUrl"]);
            
        }

        /// <summary>
        /// Tạo url thanh toán
        /// </summary>
        /// <param name="money">Số tiền phải thanh toán</param>
        /// <param name="description">Mô tả giao dịch</param>
        /// <returns></returns>
        //[HttpGet("CreatePaymentUrl")]
        //public ActionResult<string> CreatePaymentUrl(double money, string description)
        //{
        //    try
        //    {
        //        var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

        //        var request = new VNPAY.NET.Models.PaymentRequest
        //        {
        //            PaymentId = DateTime.Now.Ticks,
        //            Money = money,
        //            Description = description,
        //            IpAddress = ipAddress,
        //            BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
        //            CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
        //            Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
        //            Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
        //        };

        //        var paymentUrl = _vnpay.GetPaymentUrl(request);

        //        return Created(paymentUrl, paymentUrl);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        /// <summary>
        /// Thực hiện hành động sau khi thanh toán. URL này cần được khai báo với VNPAY để API này hoạt đồng (ví dụ: http://localhost:1234/api/Vnpay/IpnAction)
        /// </summary>
        /// <returns></returns>
        [HttpGet("IpnAction")]
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

        ///// <summary>
        ///// Trả kết quả thanh toán về cho người dùng
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("Callback")]
        //public ActionResult<PaymentResult> Callback()
        //{
        //    if (Request.QueryString.HasValue)
        //    {
        //        try
        //        {
        //            var paymentResult = _vnpay.GetPaymentResult(Request.Query);

        //            if (paymentResult.IsSuccess)
        //            {
        //                return Ok(paymentResult);
        //            }

        //            return BadRequest(paymentResult);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //    return NotFound("Không tìm thấy thông tin thanh toán.");
        //}
        // Phương thức Callback trả về kết quả thanh toán dưới dạng JSON
        //[HttpGet("Callback")]
        //public ActionResult<PaymentResult> Callback()
        //{
        //    if (Request.QueryString.HasValue)
        //    {
        //        try
        //        {
        //            var paymentResult = _vnpay.GetPaymentResult(Request.Query);

        //            if (paymentResult.IsSuccess)
        //            {
        //                // Trả về URL của trang thành công cho FE
        //                return Ok(new { Status = "Success", RedirectUrl = "https://phamdangtuc-001-site1.ntempurl.com/swagger/index.html" });
        //            }

        //            // Trả về URL của trang thất bại cho FE
        //            return Ok(new { Status = "Failed", RedirectUrl = "https://phamdangtuc-001-site1.ntempurl.com/swagger/index.html" });
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(new { Status = "Error", Message = ex.Message });
        //        }
        //    }

        //    return NotFound("Không tìm thấy thông tin thanh toán.");
        //}

        [HttpGet("CreatePaymentUrl")]
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
                base64 = base64Image,
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
            string imageUrl = await cloudinaryService.UploadBase64ImageAsync(base64Image, fileName);

            return Ok(new
            {
                base64 = base64Image,
                fullData = apiResponse,
                imageUrl = imageUrl
            });
        }



        //[HttpGet("Callback")]
        //public async Task<ActionResult<ResponseDTO>> Callback()
        //{
        //    //if (!Request.QueryString.HasValue)
        //    //{
        //    //    return NotFound(new ResponseDTO(404, "Không tìm thấy thông tin thanh toán."));
        //    //}

        //    try
        //    {
        //        var paymentResult = _vnpay.GetPaymentResult(Request.Query);
        //        if (!paymentResult.IsSuccess)
        //        {
        //            return BadRequest(new ResponseDTO(400, "Payment failed", new { RedirectUrl = "https://yourfrontend.com/payment-failed" }));
        //        }

        //        //var orderId = (int)paymentResult.PaymentId;
        //        int orderId = 6;
        //        var existingOrder = await _orderService.GetOrderByIdAsync(orderId);
        //        if (existingOrder == null)
        //        {
        //            return BadRequest(new ResponseDTO(400, $"OrderId {orderId} does not exist."));
        //        }

        //        var orderStageDto = new OrderStageCreateDTO
        //        {
        //            OrderId = orderId,
        //            OrderStageName = "Purchased",
        //            UpdatedDate = DateTime.UtcNow
        //        };

        //        var response = await _orderStageService.CreateOrderStageAsync(orderStageDto);
        //        if (response.Status != 201)
        //        {
        //            return BadRequest(response);
        //        }

        //        return Ok(new ResponseDTO(200, "Payment success", new { RedirectUrl = "https://yourfrontend.com/payment-success" }));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ResponseDTO(500, "Internal server error", ex.Message));
        //    }
        //}




        //#region test call back
        //[HttpGet("Callback")]
        //public async Task<ActionResult<ResponseDTO>> Callback()
        //{
        //    try
        //    {
        //        var paymentResult = _vnpay.GetPaymentResult(Request.Query);
        //        if (!paymentResult.IsSuccess)
        //        {
        //            return BadRequest(new ResponseDTO(400, "Payment failed", new { RedirectUrl = "https://swd-fe-nine.vercel.app/payment-failed" }));
        //        }
        //        // Extract orderId from vnp_TxnRef
        //        if (!int.TryParse(Request.Query["vnp_TxnRef"], out int orderId))
        //        {
        //            return BadRequest(new ResponseDTO(400, "Invalid Order ID"));
        //        }

        //        // Extract payment data
        //        var paymentDto = new PaymentAPIVNP
        //        {
        //            OrderId = orderId, // Extract from vnp_OrderInfo or vnp_TxnRef
        //            Amount = decimal.Parse(Request.Query["vnp_Amount"]) / 100,
        //            BankCode = Request.Query["vnp_BankCode"],
        //            BankTranNo = Request.Query["vnp_BankTranNo"],
        //            CardType = Request.Query["vnp_CardType"],
        //            OrderInfo = Request.Query["vnp_OrderInfo"],
        //            PayDate = Request.Query["vnp_PayDate"],
        //            ResponseCode = Request.Query["vnp_ResponseCode"],
        //            TransactionNo = Request.Query["vnp_TransactionNo"],
        //            TransactionStatus = Request.Query["vnp_TransactionStatus"],
        //            TxnRef = Request.Query["vnp_TxnRef"],
        //            SecureHash = Request.Query["vnp_SecureHash"],
        //            CreatedAt = DateTime.UtcNow
        //        };
        //        TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
        //        DateTime nowVietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

        //        // Convert DTO to Entity (Payment)
        //        var paymentEntity = new Payment
        //        {
        //            OrderId = paymentDto.OrderId,
        //            TotalAmount = paymentDto.Amount,
        //            DepositPaid = paymentDto.Amount, // Adjust if needed
        //            DepositAmount = paymentDto.Amount, // Adjust if needed
        //            //PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
        //            PaymentDate = nowVietnamTime
        //        };

        //        // Save payment details
        //        await _paymentService.SavePaymentAsync(paymentEntity);

        //        // Update order status
        //        var existingOrder = await _orderService.GetOrderByIdAsync(paymentDto.OrderId);
        //        if (existingOrder == null)
        //        {
        //            return BadRequest(new ResponseDTO(400, $"OrderId {paymentDto.OrderId} does not exist."));
        //        }

        //        #region Handle order stage



        //        //var orderStageDto = new OrderStageCreateDTO
        //        //{
        //        //    OrderId = paymentDto.OrderId,
        //        //    OrderStageName = "Purchased",
        //        //    UpdatedDate = DateTime.UtcNow
        //        //};

        //        //var response = await _orderStageService.CreateOrderStageAsync(orderStageDto);
        //        //if (response.Status != 201)
        //        //{
        //        //    return BadRequest(response);
        //        //}

        //        // Check if the order already has a payment stage
        //        var response = await _orderStageService.GetOrderStageByOrderIdAsync(paymentDto.OrderId);
        //        OrderStageResponseDTO? existingOrderStageDto = null;

        //        if (response.Status == 200 && response.Data is OrderStageResponseDTO orderStageData)
        //        {
        //            existingOrderStageDto = orderStageData;
        //        }
        //        if (existingOrderStageDto != null)
        //        {
        //            // Convert DTO to OrderStage model
        //            var existingOrderStage = new OrderStage
        //            {
        //                OrderStageId = existingOrderStageDto.OrderStageId,
        //                OrderId = existingOrderStageDto.OrderId,
        //                OrderStageName = "Purchased",  // Updating the stage
        //                UpdatedDate = DateTime.UtcNow
        //            };

        //            var updateResponse = await _orderStageService.UpdateOrderStageAsync(existingOrderStage);
        //            if (updateResponse.Status != 200) // Assuming 200 means successful update
        //            {
        //                return BadRequest(updateResponse);
        //            }
        //        }
        //        else
        //        {
        //            // If no order stage exists, create a new one
        //            var orderStageDto = new OrderStageCreateDTO
        //            {
        //                OrderId = paymentDto.OrderId,
        //                OrderStageName = "Purchased",
        //                UpdatedDate = DateTime.UtcNow
        //            };

        //            var createResponse = await _orderStageService.CreateOrderStageAsync(orderStageDto);
        //            if (createResponse.Status != 201)
        //            {
        //                return BadRequest(response);
        //            }
        //        }
        //        #endregion



        //        return Ok(new ResponseDTO(200, "Payment success", new { RedirectUrl = "https://swd-fe-nine.vercel.app/payment-success" }));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ResponseDTO(500, "Internal server error", ex.Message));
        //    }
        //}
        //#endregion
        [HttpGet("Callback")]
        public async Task<IActionResult> Callback()
        {
            try
            {
                var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                if (!paymentResult.IsSuccess)
                {
                    // Chuyển hướng đến trang thất bại nếu thanh toán không thành công
                    return Redirect("https://swd-fe-nine.vercel.app/payment-failed");
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
                return Redirect("https://swd-fe-nine.vercel.app/payment-success");
            }
            catch (Exception ex)
            {
                // Chuyển hướng đến trang lỗi nếu có exception
                return Redirect("https://swd-fe-nine.vercel.app/payment-error?message=" + WebUtility.UrlEncode(ex.Message));
            }
        }


        [HttpGet("MoMo/CreatePaymentUrl")]
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

        [HttpGet("MoMo/Callback")]
        public async Task<IActionResult> Callback([FromBody] MoMoCallbackModel callback)
        {
            try
            {
                if (callback.ResultCode != 0)
                    return Redirect("https://swd-fe-nine.vercel.app/payment-failed");

                if (!int.TryParse(callback.OrderId, out int orderId))
                    return BadRequest("Invalid Order ID");

                var payment = new Payment
                {
                    OrderId = orderId,
                    TotalAmount = callback.Amount,
                    DepositPaid = callback.Amount,
                    DepositAmount = callback.Amount,
                    PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
                };

                await _paymentService.SavePaymentAsync(payment);

                var existingOrder = await _orderService.GetOrderByIdAsync(orderId);
                if (existingOrder == null)
                    return BadRequest($"OrderId {orderId} does not exist.");

                var response = await _orderStageService.GetOrderStageByOrderIdAsync(orderId);
                OrderStageResponseDTO? existingOrderStageDto = null;

                if (response.Status == 200 && response.Data is OrderStageResponseDTO stageData)
                    existingOrderStageDto = stageData;

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
                        return BadRequest(updateResponse);
                }
                else
                {
                    var orderStageDto = new OrderStageCreateDTO
                    {
                        OrderId = orderId,
                        OrderStageName = "Purchased",
                        UpdatedDate = DateTime.UtcNow
                    };

                    var createResponse = await _orderStageService.CreateOrderStageAsync(orderStageDto);
                    if (createResponse.Status != 201)
                        return BadRequest(response);
                }

                return Ok(new { message = "MoMo payment processed successfully" });
            }
            catch (Exception ex)
            {
                return Redirect("https://swd-fe-nine.vercel.app/payment-error?message=" + WebUtility.UrlEncode(ex.Message));
            }
        }



    }
}