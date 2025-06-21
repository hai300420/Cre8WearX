using _2_Service.Service;
using _2_Service.ThirdPartyService;
using AutoMapper;
using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.ResponseDTO.ResponseDTO;

namespace _1_SPR25_SWD392_ClothingCustomization.Controllers
{
    [Route("api/customizeproducts")]
    [ApiController]
    public class CustomizeProductController : Controller
    {
        private readonly ICustomizeProductService _customizeProductService;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinaryService;

        public CustomizeProductController(ICustomizeProductService customizeProductService, IMapper mapper, CloudinaryService cloudinaryService)
        {
            _customizeProductService = customizeProductService;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CustomizeProductResponseDTO>>> GetAll()
        //{
        //    return Ok(await _customizeProductService.GetAllCustomizeProducts());
        //}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomizeProductResponseDTO>>> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var products = await _customizeProductService.GetAllCustomizeProducts(pageNumber, pageSize);
            return Ok(products);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CustomizeProduct>> GetById(int id)
        {
            var customizeProduct = await _customizeProductService.GetCustomizeProductById(id);
            if (customizeProduct == null)
                return NotFound();
            return Ok(customizeProduct);
        }

        //[HttpPost]
        //public async Task<ActionResult> Create([FromBody] CustomizeProduct customizeProduct)
        //{
        //    await _customizeProductService.AddCustomizeProduct(customizeProduct);
        //    return CreatedAtAction(nameof(GetById), new { id = customizeProduct.CustomizeProductId }, customizeProduct);
        //}
        // POST api/customizeproduct

        //[HttpPost]
        //public async Task<ActionResult> Create([FromBody] CreateCustomizeProductDTO createCustomizeProductDTO)
        //{
        //    // Kiểm tra dữ liệu hợp lệ
        //    if (createCustomizeProductDTO == null)
        //    {
        //        return BadRequest("Invalid data.");
        //    }

        //    // Ánh xạ DTO thành entity CustomizeProduct
        //    var customizeProduct = _mapper.Map<CustomizeProduct>(createCustomizeProductDTO);

        //    // Gọi service để lưu dữ liệu vào cơ sở dữ liệu
        //    await _customizeProductService.AddCustomizeProduct(customizeProduct);

        //    // Trả về kết quả khi tạo thành công
        //    return CreatedAtAction(nameof(GetById), new { id = customizeProduct.CustomizeProductId }, customizeProduct);
        //}

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CustomizeProduct customizeProduct)
        {
            if (id != customizeProduct.CustomizeProductId)
                return BadRequest();
            await _customizeProductService.UpdateCustomizeProduct(customizeProduct);
            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _customizeProductService.DeleteCustomizeProduct(id);
            return NoContent();
        }



        [HttpGet("customization-count")]
        public async Task<IActionResult> GetProductCustomizationCounts()
        {
            var result = await _customizeProductService.GetProductCustomizationCounts();
            return Ok(result);
        }

        //[HttpPost("create-with-order")]
        //public async Task<IActionResult> CreateCustomizeProductWithOrder([FromBody] CreateCustomizeDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        // Upload image if base64 is provided
        //        if (!string.IsNullOrEmpty(dto.Base64Image))
        //        {
        //            var fileName = $"custom_{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid()}.jpg";
        //            var uploadedUrl = await _cloudinaryService.UploadBase64ImageAsync(dto.Base64Image, fileName);
        //            dto.FullImage = uploadedUrl; // assign uploaded image url to FullImage field
        //        }

        //        // Pass updated image URL to the service
        //        var result = await _customizeProductService.CreateCustomizeProductWithOrderAsync(dto);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        [HttpPost("create-with-order")]
        public async Task<IActionResult> CreateCustomizeProductWithOrder([FromBody] CreateCustomizeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _customizeProductService.CreateCustomizeProductWithOrderAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("customize/{id}")]
        public async Task<IActionResult> UpdateCustomizeProduct(int id, [FromBody] UpdateCustomizeProductDto dto)
        {
            var customizeProduct = await _customizeProductService.GetCustomizeProductById(id);
            if (customizeProduct == null)
                return NotFound("Customize product not found");

            // Update image if provided
            if (!string.IsNullOrEmpty(dto.Base64Image))
            {
                var fileName = $"custom_update_{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid()}.jpg";
                var uploadedUrl = await _cloudinaryService.UploadBase64ImageAsync(dto.Base64Image, fileName);
                customizeProduct.FullImage = uploadedUrl;
            }

            // Update other fields
            if (!string.IsNullOrEmpty(dto.ShirtColor))
                customizeProduct.ShirtColor = dto.ShirtColor;

            if (!string.IsNullOrEmpty(dto.Description))
                customizeProduct.Description = dto.Description;

            if (dto.Price.HasValue)
                customizeProduct.Price = dto.Price.Value;

            await _customizeProductService.UpdateCustomizeProduct(customizeProduct);
            return NoContent();
        }



    }

}
