using BusinessObject.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _2_Service.Service.IService
{
    public interface ICategoryService
    {
        Task<ResponseDTO> GetListCategoriesAsync();
        Task<ResponseDTO> GetCategoryByIdAsync(int categoryId);
        Task<ResponseDTO> GetProductsByCategoryIdAsync(int categoryId); // Lấy sản phẩm theo Category
        Task<ResponseDTO> CreateCategoryAsync(CategoryCreateDTO categoryDto);
        Task<ResponseDTO> UpdateCategoryAsync(CategoryUpdateDTO categoryDto);
        Task<ResponseDTO> DeleteCategoryAsync(int categoryId);
    }
}
