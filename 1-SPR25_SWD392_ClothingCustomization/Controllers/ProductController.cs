    using BusinessObject;
using BusinessObject.ResponseDTO;
using Microsoft.AspNetCore.Mvc;
using Service.Service;
using static BusinessObject.RequestDTO.RequestDTO;

namespace SPR25_SWD392_ClothingCustomization.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListProduct()
        {
            try
            {
                var result = await _productService.GetListProductsAsync();
                return result.Status == Const.SUCCESS_READ_CODE
                    ? Ok(result)
                    : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message));
            }
        }
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableProducts()
        {
            try
            {
                var result = await _productService.GetAvailableProductsAsync();
                return result.Status == Const.SUCCESS_READ_CODE
                    ? Ok(result)
                    : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new ResponseDTO(Const.ERROR_EXCEPTION, "Invalid ID"));
                var result = await _productService.GetProductByIdAsync(id);
                return result.Status == Const.SUCCESS_READ_CODE
                    ? Ok(result)
                    : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _productService.CreateProductAsync(productDto);
                return result.Status == Const.SUCCESS_CREATE_CODE
                    ? CreatedAtAction(nameof(GetProductById), new { id = result.Data }, result)
                    : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message));
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductUpdateDTO productDto)
        {
            try
            {
                if (productDto.ProductId <= 0)
                    return BadRequest(new ResponseDTO(Const.ERROR_EXCEPTION, "Invalid Product ID"));

                var result = await _productService.UpdateProductAsync(productDto);
                return result.Status == Const.SUCCESS_UPDATE_CODE
                    ? Ok(result)
                    : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new ResponseDTO(Const.ERROR_EXCEPTION, "Invalid ID"));
                var result = await _productService.DeleteProductAsync(id);
                return result.Status == Const.SUCCESS_DELETE_CODE
                    ? Ok(result)
                    : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message));
            }
        }
    }
}

