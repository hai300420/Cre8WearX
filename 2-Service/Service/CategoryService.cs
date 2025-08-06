using AutoMapper;
using BusinessObject.Model;
using BusinessObject;
using BusinessObject.ResponseDTO;
using Service;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.ResponseDTO.ResponseDTO;
using Microsoft.EntityFrameworkCore;
using _2_Service.Service.IService;

namespace Service.Service
{
    //public interface ICategoryService
    //{
    //    Task<ResponseDTO> GetListCategoriesAsync();
    //    Task<ResponseDTO> GetCategoryByIdAsync(int categoryId);
    //    Task<ResponseDTO> GetProductsByCategoryIdAsync(int categoryId); // Lấy sản phẩm theo Category
    //    Task<ResponseDTO> CreateCategoryAsync(CategoryCreateDTO categoryDto);
    //    Task<ResponseDTO> UpdateCategoryAsync(CategoryUpdateDTO categoryDto);
    //    Task<ResponseDTO> DeleteCategoryAsync(int categoryId);
    //}

    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // 1. Lấy danh sách sản phẩm theo Category
        public async Task<ResponseDTO> GetProductsByCategoryIdAsync(int categoryId)
        {
            var category = await _unitOfWork.CategoryRepository
                .GetAll()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && !c.IsDeleted);

            if (category == null)
                return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "Category not found");

            var products = category.Products.Where(p => !p.IsDeleted).ToList();
            var result = _mapper.Map<List<ProductListDTO>>(products);

            return new ResponseDTO(Const.SUCCESS_READ_CODE, "Products retrieved successfully", result);
        }

        // 2. Kiểm tra sản phẩm khi xóa Category (Soft Delete)
        public async Task<ResponseDTO> DeleteCategoryAsync(int categoryId)
        {
            var category = await _unitOfWork.CategoryRepository
                .GetAll()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
                return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "Category not found");

            // Soft delete các sản phẩm thuộc category
            foreach (var product in category.Products)
            {
                product.IsDeleted = true;
                _unitOfWork.ProductRepository.Update(product);
            }

            // Soft delete category
            category.IsDeleted = true;
            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseDTO(Const.SUCCESS_DELETE_CODE, "Category and its products deleted successfully");
        }

        // 3. Update sản phẩm khi cập nhật Category
        public async Task<ResponseDTO> UpdateCategoryAsync(CategoryUpdateDTO categoryDto)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryDto.CategoryId);
            if (category == null)
                return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "Category not found");

            _mapper.Map(categoryDto, category);
            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, "Category updated successfully");
        }
        public async Task<ResponseDTO> CreateCategoryAsync(CategoryCreateDTO categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                await _unitOfWork.CategoryRepository.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDTO(
                    Const.SUCCESS_CREATE_CODE,
                    "Category created successfully",
                    category.CategoryId);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        // 2. Lấy danh sách các Category
        public async Task<ResponseDTO> GetListCategoriesAsync()
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository
                    .GetAll()
                    .Include(c => c.Products)
                    .Where(c => !c.IsDeleted)
                    .ToListAsync();

                if (!categories.Any())
                    return new ResponseDTO(Const.SUCCESS_READ_CODE, "No categories found");

                var result = _mapper.Map<List<BusinessObject.RequestDTO.RequestDTO.CategoryListDTO>>(categories);
                foreach (var category in result)
                {
                    category.ProductCount = categories
                        .First(c => c.CategoryId == category.CategoryId)
                        .Products.Count;
                }

                return new ResponseDTO(Const.SUCCESS_READ_CODE, "Categories retrieved successfully", result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        // 3. Lấy chi tiết một Category theo ID
        public async Task<ResponseDTO> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _unitOfWork.CategoryRepository
                .GetAll()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && !c.IsDeleted);

            if (category == null)
                return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "Category not found");

            var result = _mapper.Map<CategoryDetailDTO>(category);
            result.ProductCount = category.Products.Count;

            return new ResponseDTO(Const.SUCCESS_READ_CODE, "Category retrieved successfully", result);
        }
    }

}



