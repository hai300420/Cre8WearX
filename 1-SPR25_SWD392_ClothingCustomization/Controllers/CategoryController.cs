using _2_Service.Service;
using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Service.Service;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _1_SPR25_SWD392_ClothingCustomization.Controllers
{
    [ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

        [HttpGet]
        public async Task<IActionResult> GetListCategories()
        {
            var result = await _categoryService.GetListCategoriesAsync();
            return result.Status == Const.SUCCESS_READ_CODE ? Ok(result) : BadRequest(result);
        }
        // Lấy danh sách sản phẩm theo Category
        [HttpGet("{categoryId}/products")]
    public async Task<IActionResult> GetProductsByCategoryId(int categoryId)
    {
        var result = await _categoryService.GetProductsByCategoryIdAsync(categoryId);
        return result.Status == Const.SUCCESS_READ_CODE ? Ok(result) : NotFound(result);
    }

 

    [HttpGet("GetById")]
    public async Task<IActionResult> GetCategoryById([FromQuery] int categoryId)
    {
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);
        return result.Status == Const.SUCCESS_READ_CODE ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO categoryDto)
    {
        var result = await _categoryService.CreateCategoryAsync(categoryDto);
        return result.Status == Const.SUCCESS_CREATE_CODE ? Ok(result) : BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDTO categoryDto)
    {
        var result = await _categoryService.UpdateCategoryAsync(categoryDto);
        return result.Status == Const.SUCCESS_UPDATE_CODE ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> DeleteCategory([FromQuery] int categoryId)
    {
        var result = await _categoryService.DeleteCategoryAsync(categoryId);
        return result.Status == Const.SUCCESS_DELETE_CODE ? Ok(result) : BadRequest(result);
    }
}

}
