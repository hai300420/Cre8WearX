using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3_Repository.IRepository;
using AutoMapper;
using BusinessObject.Model;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Repository.IRepository;
using Service;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.ResponseDTO.ResponseDTO;
using _2_Service.ThirdPartyService;

namespace _2_Service.Service
{
    public interface ICustomizeProductService
    {
        Task<IEnumerable<CustomizeProduct>> GetAllCustomizeProducts();
        Task<CustomizeProduct> GetCustomizeProductById(int id);
        Task AddCustomizeProduct(CustomizeProduct customizeProduct);
        Task UpdateCustomizeProduct(CustomizeProduct customizeProduct);
        Task DeleteCustomizeProduct(int id);
        Task<List<ProductCustomizationCountDto>> GetProductCustomizationCounts();
        Task<IEnumerable<CustomizeProduct>> GetAllCustomizeProductsAsync();
        Task<CustomizeProduct> GetCustomizeProductByIdAsync(int id);
        Task<IEnumerable<CustomizeProduct>> GetCustomizeProductsByCurrentUserAsync(int userId);
        Task UpdateCustomizeProductAsync(CustomizeProduct product);
        Task DeleteCustomizeProductAsync(int id);
        Task<CustomizeProduct> CreateCustomizeProductAsync(CustomizeProduct product);
        Task<CustomizeProductWithOrderResponse> CreateCustomizeProductWithOrderAsync(CreateCustomizeDto dto);
        Task<IEnumerable<CustomizeProduct>> GetAllCustomizeProducts(int pageNumber, int pageSize);


    }
    public class CustomizeProductService : ICustomizeProductService
    {
        private readonly ICustomizeProductRepository _customizeProductRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderStageRepository _orderStageRepo;
        private readonly IProductRepository _productRepository;
        private readonly CloudinaryService _cloudinaryService;
        public CustomizeProductService(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ICustomizeProductRepository customizeProductRepo,
        IOrderRepository orderRepo,
        IOrderStageRepository orderStageRepo,
        CloudinaryService cloudinaryService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _customizeProductRepository = customizeProductRepo;
            _orderRepo = orderRepo;
            _orderStageRepo = orderStageRepo;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IEnumerable<CustomizeProduct>> GetAllCustomizeProducts()
        {
            return await _customizeProductRepository.GetAllAsync();
        }

        public async Task<CustomizeProduct> GetCustomizeProductById(int id)
        {
            return await _customizeProductRepository.GetByIdAsync(id);
        }

        public async Task AddCustomizeProduct(CustomizeProduct customizeProduct)
        {
            await _customizeProductRepository.AddAsync(customizeProduct);
        }

        public async Task UpdateCustomizeProduct(CustomizeProduct customizeProduct)
        {
            await _customizeProductRepository.UpdateAsync(customizeProduct);
        }

        public async Task DeleteCustomizeProduct(int id)
        {
            await _customizeProductRepository.DeleteAsync(id);
        }



        public async Task<List<ProductCustomizationCountDto>> GetProductCustomizationCounts()
        {
            return await _customizeProductRepository.GetProductCustomizationCounts();
        }

        public async Task<IEnumerable<CustomizeProduct>> GetAllCustomizeProductsAsync()
        {
            return await _customizeProductRepository.GetAllWithProductAndUserAsync();
        }

        public async Task<CustomizeProduct> GetCustomizeProductByIdAsync(int id)
        {
            return await _customizeProductRepository.GetWithElementsAsync(id);
        }

        public async Task<IEnumerable<CustomizeProduct>> GetCustomizeProductsByCurrentUserAsync(int userId)
        {
            return await _customizeProductRepository.GetByUserIdAsync(userId);
        }

        public async Task UpdateCustomizeProductAsync(CustomizeProduct product)
        {
            await _customizeProductRepository.UpdateAsync(product);
        }

        public async Task DeleteCustomizeProductAsync(int id)
        {
            var product = await _customizeProductRepository.GetByIdAsync(id);
            if (product != null)
            {
                await _customizeProductRepository.RemoveAsync(product);
            }
        }

        public async Task<CustomizeProduct> CreateCustomizeProductAsync(CustomizeProduct product)
        {
            await _customizeProductRepository.CreateAsync(product);
            return product;
        }
        //public async Task<CustomizeProductWithOrderResponse> CreateCustomizeProductWithOrderAsync(CreateCustomizeDto dto)
        //{
        //    // Start transaction
        //    await _unitOfWork.BeginTransactionAsync();

        //    try
        //    {
        //        // 1. Get the product to get its image and price
        //        var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
        //        if (product == null)
        //        {
        //            throw new Exception($"Product with ID {dto.ProductId} not found");
        //        }

        //        // 2. Create CustomizeProduct using the tuple mapping
        //        var customizeProduct = _mapper.Map<CustomizeProduct>((dto, product));

        //        await _customizeProductRepository.AddAsync(customizeProduct);
        //        await _unitOfWork.SaveChangesAsync();

        //        // 3. Create Order
        //        var order = new Order
        //        {
        //            CustomizeProductId = customizeProduct.CustomizeProductId,
        //            OrderDate = DateTime.UtcNow,
        //            DeliveryDate = dto.DeliveryDate,
        //            RecipientName = dto.RecipientName,
        //            DeliveryAddress = dto.DeliveryAddress,
        //            ShippingMethod = dto.ShippingMethod,
        //            ShippingFee = (double)dto.ShippingFee,
        //            Notes = dto.Notes,
        //            Price = product.Price, // Use product's price
        //            Quantity = dto.Quantity,
        //            TotalPrice = (product.Price * dto.Quantity) + dto.ShippingFee
        //        };

        //        await _orderRepo.AddAsync(order);
        //        await _unitOfWork.SaveChangesAsync();

        //        // 4. Create initial OrderStage
        //        var orderStage = new OrderStage
        //        {
        //            OrderId = order.OrderId,
        //            OrderStageName = "Tạo thành công",
        //            UpdatedDate = DateTime.UtcNow
        //        };

        //        await _orderStageRepo.AddOrderStageAsync(orderStage);
        //        await _unitOfWork.SaveChangesAsync();

        //        // Commit transaction
        //        await _unitOfWork.CommitAsync();

        //        return new CustomizeProductWithOrderResponse
        //        {
        //            CustomizeProductId = customizeProduct.CustomizeProductId,
        //            OrderId = order.OrderId,
        //            OrderStageId = orderStage.OrderStageId,
        //            Message = "Customize product and order created successfully"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await _unitOfWork.RollbackAsync();
        //        throw new Exception($"Error creating customize product and order: {ex.Message}", ex);
        //    }
        //}
        //public async Task<CustomizeProductWithOrderResponse> CreateCustomizeProductWithOrderAsync(CreateCustomizeDto dto)
        //{
        //    // Start transaction
        //    await _unitOfWork.BeginTransactionAsync();

        //    try
        //    {
        //        // 1. Get the product to get its image and price
        //        var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
        //        if (product == null)
        //        {
        //            throw new Exception($"Product with ID {dto.ProductId} not found");
        //        }

        //        // 2. Create CustomizeProduct using the tuple mapping
        //        var customizeProduct = _mapper.Map<CustomizeProduct>((dto, product));

        //        await _customizeProductRepository.AddAsync(customizeProduct);
        //        await _unitOfWork.SaveChangesAsync();

        //        // Calculate base price without shipping
        //        decimal basePrice = (decimal)(product.Price * dto.Quantity);

        //        // Apply quantity-based discounts
        //        decimal totalPrice = dto.Quantity switch
        //        {
        //            >= 10 and <= 20 => basePrice,
        //            >= 21 and <= 30 => basePrice * 0.92m,  // 8% discount
        //            >= 31 and <= 40 => basePrice * 0.90m,  // 10% discount
        //            >= 41 and <= 50 => basePrice * 0.85m,  // 15% discount
        //            >= 51 and <= 60 => basePrice * 0.83m,  // 17% discount
        //            >= 61 and <= 80 => basePrice * 0.80m,  // 20% discount
        //            >= 81 and <= 99 => basePrice * 0.78m,  // 22% discount
        //            >= 100 => basePrice * 0.75m,           // 25% discount
        //            _ => basePrice                         // No discount for quantities < 10
        //        };

        //        // Add shipping fee to the discounted price
        //        totalPrice += dto.ShippingFee;

        //        // 3. Create Order
        //        var order = new Order
        //        {
        //            CustomizeProductId = customizeProduct.CustomizeProductId,
        //            OrderDate = DateTime.UtcNow,
        //            DeliveryDate = dto.DeliveryDate,
        //            RecipientName = dto.RecipientName,
        //            DeliveryAddress = dto.DeliveryAddress,
        //            ShippingMethod = dto.ShippingMethod,
        //            ShippingFee = (double)dto.ShippingFee,
        //            Notes = dto.Notes,
        //            Price = product.Price,
        //            Quantity = dto.Quantity,
        //            TotalPrice = totalPrice
        //        };

        //        await _orderRepo.AddAsync(order);
        //        await _unitOfWork.SaveChangesAsync();

        //        // 4. Create initial OrderStage
        //        var orderStage = new OrderStage
        //        {
        //            OrderId = order.OrderId,
        //            OrderStageName = "Tạo thành công",
        //            UpdatedDate = DateTime.UtcNow
        //        };

        //        await _orderStageRepo.AddOrderStageAsync(orderStage);
        //        await _unitOfWork.SaveChangesAsync();

        //        // Commit transaction
        //        await _unitOfWork.CommitAsync();

        //        return new CustomizeProductWithOrderResponse
        //        {
        //            CustomizeProductId = customizeProduct.CustomizeProductId,
        //            OrderId = order.OrderId,
        //            OrderStageId = orderStage.OrderStageId,
        //            Message = "Customize product and order created successfully",
        //            TotalPrice = totalPrice  // Include the calculated total price in response
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await _unitOfWork.RollbackAsync();
        //        throw new Exception($"Error creating customize product and order: {ex.Message}", ex);
        //    }
        //}
        public async Task<IEnumerable<CustomizeProduct>> GetAllCustomizeProducts(int pageNumber, int pageSize)
        {
            return await _customizeProductRepository.GetAllAsync(pageNumber, pageSize);
        }

        public async Task<CustomizeProductWithOrderResponse> CreateCustomizeProductWithOrderAsync(CreateCustomizeDto dto)
        {
            // Start transaction
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Get the product to get its image, price and check stock
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
                if (product == null)
                {
                    throw new Exception($"Không tìm thấy sản phẩm với ID {dto.ProductId}");
                }

                // Check if enough stock is available
                if (product.StockInStorage < dto.Quantity)
                {
                    throw new Exception($"Số lượng tồn kho không đủ. Hiện có: {product.StockInStorage}, yêu cầu: {dto.Quantity}");
                }

                // 2. Create CustomizeProduct using the tuple mapping
                var customizeProduct = _mapper.Map<CustomizeProduct>((dto, product));

                // customizeProduct.FullImage = await _cloudinaryService.UploadImageFromUrlAsync(product.Image,"product_copy.png");
                // Upload base64 image from frontend canvas
                // customizeProduct.FullImage = await _cloudinaryService.UploadBase64ImageAsync(dto.Base64Image, "custom_design");
                // Upload image only if base64 is provided
                if (string.IsNullOrWhiteSpace(dto.Base64Image) && string.IsNullOrWhiteSpace(dto.FullImage))
                {
                    throw new Exception("Bạn phải cung cấp ảnh thiết kế (base64 hoặc link).");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dto.Base64Image))
                    {
                        customizeProduct.FullImage = await _cloudinaryService.UploadBase64ImageAsync(dto.Base64Image, "custom_design");
                    }
                    else if (!string.IsNullOrWhiteSpace(dto.FullImage))
                    {
                        customizeProduct.FullImage = dto.FullImage; // Use the provided image URL directly
                    }
                }
                
                

                await _customizeProductRepository.AddAsync(customizeProduct);
                await _unitOfWork.SaveChangesAsync();

                // Calculate price with quantity discounts
                decimal basePrice = (decimal)(product.Price * dto.Quantity);
                decimal totalPrice = dto.Quantity switch
                {
                    >= 10 and <= 20 => basePrice,
                    >= 21 and <= 30 => basePrice * 0.92m,
                    >= 31 and <= 40 => basePrice * 0.90m,
                    >= 41 and <= 50 => basePrice * 0.85m,
                    >= 51 and <= 60 => basePrice * 0.83m,
                    >= 61 and <= 80 => basePrice * 0.80m,
                    >= 81 and <= 99 => basePrice * 0.78m,
                    >= 100 => basePrice * 0.75m,
                    _ => basePrice
                };

                // Add shipping fee
                totalPrice += dto.ShippingFee;

                // 3. Create Order
                var order = new Order
                {
                    CustomizeProductId = customizeProduct.CustomizeProductId,
                    OrderDate = DateTime.UtcNow,
                    DeliveryDate = dto.DeliveryDate,
                    RecipientName = dto.RecipientName,
                    DeliveryAddress = dto.DeliveryAddress,
                    ShippingMethod = dto.ShippingMethod,
                    ShippingFee = (double)dto.ShippingFee,
                    Notes = dto.Notes,
                    Price = product.Price,
                    Quantity = dto.Quantity,
                    TotalPrice = totalPrice
                };

                await _orderRepo.AddAsync(order);

                // 4. Update product stock
                product.StockInStorage -= dto.Quantity;
                _unitOfWork.ProductRepository.Update(product);

                await _unitOfWork.SaveChangesAsync();

                // 5. Create initial OrderStage
                var orderStage = new OrderStage
                {
                    OrderId = order.OrderId,
                    OrderStageName = "Đã tạo đơn",
                    UpdatedDate = DateTime.UtcNow
                };

                await _orderStageRepo.AddOrderStageAsync(orderStage);
                await _unitOfWork.SaveChangesAsync();

                // Commit transaction
                await _unitOfWork.CommitAsync();

                return new CustomizeProductWithOrderResponse
                {
                    CustomizeProductId = customizeProduct.CustomizeProductId,
                    ProductId = product.ProductId,
                    ShirtColor = customizeProduct.ShirtColor,
                    OrderId = order.OrderId,
                    TotalPrice = (decimal)order.TotalPrice,
                    OrderStatus = orderStage.OrderStageName,
                    OrderDate = (DateTime)order.OrderDate,
                    OrderStageId = orderStage.OrderStageId,
                    Message = "Tạo sản phẩm tùy chỉnh và đơn hàng thành công",
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception($"Lỗi khi tạo đơn hàng: {ex.Message}", ex);
            }
        }
    }

}
