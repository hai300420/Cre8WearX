using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2_Service.Service.IService;
using _2_Service.ThirdPartyService;
using AutoMapper;
using BusinessObject;
using BusinessObject.Model;
using BusinessObject.ResponseDTO;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.ResponseDTO.ResponseDTO;

namespace Service.Service
{
    //public interface IProductService
    //{
    //    Task<ResponseDTO> GetListProductsAsync();
    //    Task<ResponseDTO> GetAvailableProductsAsync();
    //    Task<ResponseDTO> GetProductByIdAsync(int id);
    //    Task<ResponseDTO> CreateProductAsync(ProductCreateDTO productDto);
    //    Task<ResponseDTO> UpdateProductAsync(ProductUpdateDTO productDto);
    //    Task<ResponseDTO> DeleteProductAsync(int id);
    //}


    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinaryService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, CloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        //public async Task<ResponseDTO> CreateProductAsync(ProductCreateDTO productDto)
        //{
        //    try
        //    {
        //        // Kiểm tra ProductName
        //        if (string.IsNullOrWhiteSpace(productDto.ProductName))
        //        {
        //            throw new ArgumentException("Product name is required.");
        //        }

        //        // Kiểm tra CategoryId có hợp lệ không
        //        var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(productDto.CategoryId);
        //        if (existingCategory == null)
        //        {
        //            throw new ArgumentException($"CategoryId {productDto.CategoryId} does not exist. Please provide a valid CategoryId.");
        //        }

        //        // Kiểm tra Price & StockInStorage
        //        if (productDto.Price == null || productDto.Price <= 0)
        //        {
        //            throw new ArgumentException("Price must be greater than 0.");
        //        }
        //        if (productDto.StockInStorage < 0)
        //        {
        //            throw new ArgumentException("Stock cannot be negative.");
        //        }

        //        // Map DTO sang Product
        //        var product = _mapper.Map<Product>(productDto);
        //        product.IsDeleted = false; // Khi tạo mới, mặc định sản phẩm chưa bị xóa

        //        // Thêm vào database
        //        await _unitOfWork.ProductRepository.AddAsync(product);
        //        await _unitOfWork.SaveChangesAsync();

        //        // Lấy lại sản phẩm vừa lưu
        //        var savedProduct = await _unitOfWork.ProductRepository.GetByIdAsync(product.ProductId);
        //        if (savedProduct == null)
        //        {
        //            throw new Exception("Failed to retrieve saved product.");
        //        }

        //        return new ResponseDTO(
        //            Const.SUCCESS_CREATE_CODE,
        //            "Product created successfully",
        //            savedProduct.ProductId);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        if (ex.InnerException != null)
        //        {
        //            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        //        }
        //        return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
        //    }
        //}

        public async Task<ResponseDTO> CreateProductAsync(ProductCreateDTO productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);

                await _unitOfWork.ProductRepository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDTO(
                    Const.SUCCESS_CREATE_CODE,
                    "Product created successfully",
                    product.ProductId);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        //public async Task<ResponseDTO> DeleteProductAsync(int id)
        //{
        //    var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        //    if (product == null) return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "Product not found");

        //    _unitOfWork.ProductRepository.Delete(product);
        //    await _unitOfWork.SaveChangesAsync();

        //    return new ResponseDTO(Const.SUCCESS_DELETE_CODE, "Product deleted successfully");
        //}
        public async Task<ResponseDTO> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null) return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "Product not found");

            // Soft delete bằng cách cập nhật trạng thái IsDeleted
            product.IsDeleted = true;
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseDTO(Const.SUCCESS_DELETE_CODE, "Product deleted successfully (soft delete)");
        }
        public async Task<ResponseDTO> GetListProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.ProductRepository.GetAllAsync();
                if (!products.Any())
                    return new ResponseDTO(Const.SUCCESS_READ_CODE, "No products found");

                var result = _mapper.Map<List<ProductListDTO>>(products);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, "Products retrieved successfully", result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<ResponseDTO> GetAvailableProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.ProductRepository.GetAllAsync(p => !p.IsDeleted); // Lấy sản phẩm chưa bị xóa
                if (!products.Any())
                    return new ResponseDTO(Const.SUCCESS_READ_CODE, "No available products found");

                var result = _mapper.Map<List<ProductListDTO>>(products);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, "Available products retrieved successfully", result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null) return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "Product not found");

            var result = _mapper.Map<ProductListDTO>(product);
            return new ResponseDTO(Const.SUCCESS_READ_CODE, "Success", result);
        }

        public async Task<ResponseDTO> UpdateProductAsync(ProductUpdateDTO productDto)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productDto.ProductId);
            if (product == null) return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "Product not found");

            _mapper.Map(productDto, product);
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, "Product updated successfully");
        }
    }
}
