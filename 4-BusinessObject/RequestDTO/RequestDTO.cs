using BusinessObject.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BusinessObject.RequestDTO
{
    public class RequestDTO
    {
        public class LoginRequestDTO
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public string Password { get; set; }
        }
        #region Khang

       
            public class CreateCustomizeDto
        {
            [Required]
            public int ProductId { get; set; }

            [Required]
            public int UserId { get; set; }

            [Required]
            public string ShirtColor { get; set; }

            public string Description { get; set; }

            // Order related fields
            [Required]
            public string RecipientName { get; set; }

            [Required]
            public string DeliveryAddress { get; set; }

            public string ShippingMethod { get; set; } = "Standard";

            [Range(0, double.MaxValue)]
            public decimal ShippingFee { get; set; } = 0;

            public string Notes { get; set; }

            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
            public int Quantity { get; set; } = 1;

            public DateTime? DeliveryDate { get; set; }
        }
        
            public class ProductCreateDTO
            {
                [Required(ErrorMessage = "Category ID is required")]
                public int CategoryId { get; set; }

                [Required(ErrorMessage = "Product name is required")]
                [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
                public string ProductName { get; set; } = null!;

                [Required(ErrorMessage = "Price is required")]
                [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
   
            public decimal Price { get; set; }

            [Required(ErrorMessage = "Stock is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Stock must be at least 1")]
            public int StockInStorage { get; set; }

                public string? Image { get; set; }
                public string? Description { get; set; }
            }
        public class ProductUpdateDTO
        {
            public int ProductId { get; set; }

            public int CategoryId { get; set; }

            [Required(ErrorMessage = "Product name is required")]
            [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
            public string ProductName { get; set; } = null!;

            [Required(ErrorMessage = "Price is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            public decimal Price { get; set; }

            [Required(ErrorMessage = "Stock is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Stock must be at least 1")]
            public int StockInStorage { get; set; }

            public string? Image { get; set; }
            public string? Description { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class CategoryCreateDTO
        {
            [Required]
            public string CategoryName { get; set; } = null!;
            public string? Description { get; set; }
        }

        public class CategoryUpdateDTO
        {
            public int CategoryId { get; set; }
            [Required]
            public string CategoryName { get; set; } = null!;
            public string? Description { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class CategoryListDTO
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = null!;
            public string? Description { get; set; }
            public bool IsDeleted { get; set; }
            public int ProductCount { get; set; }
        }

        // DesignElement
        public class DesignElementCreateDTO
        {
            [Required]
            public int DesignAreaId { get; set; }  // Chọn từ danh sách DesignArea (Dropdown)

            [Required]
            public int CustomizeProductId { get; set; }  // Chọn từ danh sách CustomizeProduct (Dropdown)

            public IFormFile? Image { get; set; }  // Upload hình ảnh

            [MaxLength(250)]
            public string? Text { get; set; }  // Văn bản thiết kế

            [MaxLength(10)]
            public string? Size { get; set; }

            [MaxLength(20)]
            public string? ColorArea { get; set; }
        }
        #endregion
        #region Hai
        public class FeedbackDTO
        {
            public int OrderId { get; set; }
            public int UserId { get; set; }
            [Required]
            public int Rating { get; set; }
            [Required]
            public string Review { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
        }
        public class RoleDTO
        {
            public string RoleName { get; set; } = string.Empty;
        }

        public class UserDTO
        {
            [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
            [StringLength(20, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3-20 ký tự")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email không được để trống")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Họ và tên không được để trống")]
            public string FullName { get; set; } = string.Empty;

            public bool Gender { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }
            [Required(ErrorMessage = "Địa chỉ không được để trống")]
            public string? Address { get; set; }

            [Required(ErrorMessage = "Số điện thoại không được để trống")]
            [RegularExpression(@"^(0[0-9]{9,10})$", ErrorMessage = "Số điện thoại không hợp lệ")]
            public string? Phone { get; set; }

            public string? Avatar { get; set; }

            public bool IsDeleted { get; set; }

            public string RoleName { get; set; } = string.Empty;
        }

        public class UserRegisterDTO
        {
            [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
            [StringLength(20, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3-20 ký tự")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email không được để trống")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mật khẩu không được để trống")]
            [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Họ và tên không được để trống")]
            public string FullName { get; set; } = string.Empty;

            public bool Gender { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }

            [Required(ErrorMessage = "Địa chỉ không được để trống")]
            public string? Address { get; set; }

            [Required(ErrorMessage = "Số điện thoại không được để trống")]
            [RegularExpression(@"^(0[0-9]{9,10})$", ErrorMessage = "Số điện thoại không hợp lệ")]
            public string? Phone { get; set; }

            public string? Avatar { get; set; }

            public string RoleName { get; set; } = "Member"; // Default role
        }

        public class UserUpdateDTO
        {
            [Required(ErrorMessage = "Họ và tên không được để trống")]
            public string FullName { get; set; }
            [Required(ErrorMessage = "Email không được để trống")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; }
            public bool Gender { get; set; }
            public DateTime DateOfBirth { get; set; }
            [Required(ErrorMessage = "Địa chỉ không được để trống")]
            public string Address { get; set; }
            [Required(ErrorMessage = "Số điện thoại không được để trống")]
            [RegularExpression(@"^(0[0-9]{9,10})$", ErrorMessage = "Số điện thoại không hợp lệ")]
            public string Phone { get; set; }
            public string Avatar { get; set; }
        }

        public class ChangePasswordDTO
        {
            [Required]
            public string Username { get; set; } = string.Empty;
            [Required(ErrorMessage = "Mật khẩu không được để trống")]
            [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
            public string Password { get; set; } = string.Empty;
            [Required(ErrorMessage = "Mật khẩu không được để trống")]
            [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
        public class GoogleLoginRequest
        {
            public string IdToken { get; set; }
        }

        public class NotificationDTO
        {

            public int UserId { get; set; }

            public string? Subject { get; set; }

            public string? Message { get; set; }

            public DateTime CreatedDate { get; set; }

            public bool IsRead { get; set; }
        }

        public class PostNotificationDTO
        {

            public int UserId { get; set; }

            public string? Subject { get; set; }

            public string? Message { get; set; }

        }

        public class PutNotificationDTO
        {
            public string? Subject { get; set; }

            public string? Message { get; set; }

        }

        public class NotificationRoleDTO
        {
            public string RoleName { get; set; }
            public string Subject { get; set; }
            public string Message { get; set; }

        }

        #endregion
        #region Hoang
        public class OrderDTO
        {
            public int CustomizeProductId { get; set; }
            public DateTime? OrderDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public string? RecipientName { get; set; }
            public string? DeliveryAddress { get; set; }
            public string? ShippingMethod { get; set; }
            public double? ShippingFee { get; set; }
            public string? Notes { get; set; }
            public decimal? Price { get; set; }
            public int? Quantity { get; set; }
            public decimal? TotalPrice { get; set; }

        }
        public class OrderCreateDTO
        {
            [Required]
            public int CustomizeProductId { get; set; }

            public DateTime? OrderDate { get; set; } = DateTime.UtcNow;
            public DateTime? DeliveryDate { get; set; }

            [Required]
            public string RecipientName { get; set; } = string.Empty;

            [Required]
            public string DeliveryAddress { get; set; } = string.Empty;

            public string? ShippingMethod { get; set; }
            public double? ShippingFee { get; set; }
            public string? Notes { get; set; }

            [Required]
            public decimal Price { get; set; }

            [Required]
            public int Quantity { get; set; }

            //public decimal? TotalPrice { get; set; }
        }

        public class OrderUpdateDTO
        {
            [Required]
            public int OrderId { get; set; }

            public int CustomizeProductId { get; set; }
            public DateTime? OrderDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public string? RecipientName { get; set; }
            public string? DeliveryAddress { get; set; }
            public string? ShippingMethod { get; set; }
            public double? ShippingFee { get; set; }
            public string? Notes { get; set; }
            public decimal? Price { get; set; }
            public int? Quantity { get; set; }
            public decimal? TotalPrice { get; set; }
        }

        public class OrderListDTO
        {
            public int OrderId { get; set; }
            public int CustomizeProductId { get; set; }
            public DateTime? OrderDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public string? RecipientName { get; set; }
            public string? DeliveryAddress { get; set; }
            public string? ShippingMethod { get; set; }
            public double? ShippingFee { get; set; }
            public string? Notes { get; set; }
            public decimal? Price { get; set; }
            public int? Quantity { get; set; }
            public decimal? TotalPrice { get; set; }
        }


        public class OrderStageCreateDTO
        {
            [Required(ErrorMessage = "OrderId is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "OrderId must be greater than 0.")]
            public int OrderId { get; set; }

            [Required(ErrorMessage = "OrderStageName is required.")]
            public string OrderStageName { get; set; } = null!; // 🔥 Dùng string thay vì enum

            public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
        }

        #endregion

        public class ProductCustomizationCountDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public int CustomizationCount { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class ProductOrderQuantityDto
        {
            public int Product_ID { get; set; }
            public string ProductName { get; set; }
            public int TotalOrderedQuantity { get; set; }
        }

        public class PaymentRequest
        {
            public long PaymentId { get; set; } // ID thanh toán, sử dụng timestamp để đảm bảo tính duy nhất
            public string Description { get; set; } = string.Empty; // Mô tả đơn hàng
            public double Money { get; set; }  // Số tiền thanh toán
            public string IpAddress { get; set; } = string.Empty; // Địa chỉ IP của khách hàng
            public string BankCode { get; set; } = "ANY"; // Mã ngân hàng, có thể là giá trị cụ thể hoặc mặc định "ANY"
            public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Ngày tạo thanh toán (UTC)
            public string Currency { get; set; } = "VND"; // Loại tiền tệ (VND, USD, v.v.)
            public string Language { get; set; } = "vi"; // Ngôn ngữ hiển thị giao diện thanh toán (vi = Tiếng Việt, en = English)

        }


        public class PaymentAPIVNP
        {
            public int Id { get; set; }
            public int OrderId { get; set; }
            public decimal Amount { get; set; }
            public string BankCode { get; set; }
            public string BankTranNo { get; set; }
            public string CardType { get; set; }
            public string OrderInfo { get; set; }
            public string PayDate { get; set; }
            public string ResponseCode { get; set; }
            public string TransactionNo { get; set; }
            public string TransactionStatus { get; set; }
            public string TxnRef { get; set; }
            public string SecureHash { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        public class PaymentMapping
        {
            public int Id { get; set; }
            public int OrderId { get; set; } // Your system's order ID
            public string VnpTxnRef { get; set; } // VNPAY's transaction reference
        }

        public class RevenueDto
        {
            public List<string> Labels { get; set; } = new();
            public List<RevenueDataset> Datasets { get; set; } = new();
        }

        public class RevenueDataset
        {
            public List<decimal> Data { get; set; } = new();
        }

    }
}
