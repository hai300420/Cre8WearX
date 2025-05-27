    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Enum;

namespace BusinessObject.ResponseDTO
{
    public class ResponseDTO
    {

        public int Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public ResponseDTO(int status, string? message, object? data = null)
        {
            Status = status;
            Message = message;
            Data = data;
        }
        public class LoginResponse
        {
            public int UserId { get; set; }
            public string UserName { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string? Phone { get; set; }
            public string? FullName { get; set; }
            public bool IsDeleted { get; set; }
            public string RoleName { get; set; }
        }
        public class UserListDTO
        {
            public int UserId { get; set; }
            public string? UserName { get; set; }
            public string? Password { get; set; }


        }
        public class ProductListDTO
        {
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
            public string? ProductName { get; set; }
            public decimal? Price { get; set; }
            public int StockInStorage { get; set; }
            public string? Image { get; set; }
            public string? Description { get; set; }
            public bool IsDeleted { get; set; }
        }
        public class FeedbackListDTO
        {
            public int FeedbackId { get; set; }
            public int OrderId { get; set; }
            public int UserId { get; set; }
            public int Rating { get; set; }
            public string Review { get; set; } = null!;
            public DateTime? CreatedDate { get; set; }
        }

        public class DesignElementDTO
        {
            public int DesignElementId { get; set; }
            public string? Image { get; set; }
            public string? Text { get; set; }
            public string? Size { get; set; }
            public string? ColorArea { get; set; }

            // Thông tin từ bảng DesignArea
            public int DesignAreaId { get; set; }
            public string AreaName { get; set; } = string.Empty;

            // Thông tin từ bảng CustomizeProduct
            public int CustomizeProductId { get; set; }
            public string ShirtColor { get; set; } = string.Empty;
            public string? FullImage { get; set; }
        }

        public class CategoryListDTO
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = null!;
            public string? Description { get; set; }
            public bool IsDeleted { get; set; }

            public int ProductCount { get; set; }
        }


        public class CategoryDetailDTO : CategoryListDTO
        {
            public ICollection<ProductListDTO> Products { get; set; } = new List<ProductListDTO>();
        }

        public class OrderStageListDTO
        {
            public int OrderStageId { get; set; }
            public int OrderId { get; set; }
            public string OrderStageName { get; set; } = null!;
            public DateTime? UpdatedDate { get; set; }
        }

        public class OrderStageResponseDTO
        {
            public int OrderStageId { get; set; }
            public int OrderId { get; set; }
            public string OrderStageName { get; set; }
            public DateTime? UpdatedDate { get; set; }
        }

        public class OrderDTO
        {
            [Required(ErrorMessage = "CustomizeProductId is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "CustomizeProductId must be greater than 0.")]
            public int CustomizeProductId { get; set; }

            [Required(ErrorMessage = "OrderDate is required.")]
            public DateTime? OrderDate { get; set; }

            public DateTime? DeliveryDate { get; set; }

            [Required(ErrorMessage = "RecipientName is required.")]
            [StringLength(100, ErrorMessage = "RecipientName must be between 3 and 100 characters.", MinimumLength = 3)]
            public string RecipientName { get; set; }

            [Required(ErrorMessage = "DeliveryAddress is required.")]
            public string DeliveryAddress { get; set; }

            public string? ShippingMethod { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "ShippingFee must be non-negative.")]
            public double? ShippingFee { get; set; }

            public string? Notes { get; set; }

            [Required(ErrorMessage = "Price is required.")]
            [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
            public decimal? Price { get; set; }

            [Required(ErrorMessage = "Quantity is required.")]
            [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
            public int? Quantity { get; set; }

            [Required(ErrorMessage = "TotalPrice is required.")]
            [Range(1, double.MaxValue, ErrorMessage = "TotalPrice must be greater than 0.")]
            public decimal? TotalPrice { get; set; }
        }
        public class CustomizeProductResponseDTO
        {
            public int CustomizeProductId { get; set; }
            public int ProductId { get; set; }
            public int UserId { get; set; }
            public string? ShirtColor { get; set; }
            public string? FullImage { get; set; }
            public string? Description { get; set; }
            public decimal Price { get; set; }
        }
        public class CustomizeProductWithOrderResponse
        {
            public int CustomizeProductId { get; set; }
            public int OrderStageId { get; set; }
            public int ProductId { get; set; }
            public string ShirtColor { get; set; }
            public int OrderId { get; set; }
            public decimal TotalPrice { get; set; }
            public string OrderStatus { get; set; }
            public DateTime OrderDate { get; set; }
            public string Message { get; set; }
        }

    }

}

