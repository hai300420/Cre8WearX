using _2_Service.Service;
using AutoMapper;
using BusinessObject.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _1_SPR25_SWD392_ClothingCustomization.Controllers
{
    [Route("api/designelements")]
    [ApiController]
    public class DesignElementController : Controller
    {
        private readonly IDesignElementService _designElementService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DesignElementController(IDesignElementService designElementService, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _designElementService = designElementService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DesignElement>>> GetAll()
        {
            return Ok(await _designElementService.GetAllDesignElements());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DesignElement>> GetById(int id)
        {
            var designElement = await _designElementService.GetDesignElementById(id);
            if (designElement == null)
                return NotFound();
            return Ok(designElement);
        }

        //[HttpPost]
        //public async Task<ActionResult> Create([FromBody] DesignElement designElement)
        //{
        //    await _designElementService.AddDesignElement(designElement);
        //    return CreatedAtAction(nameof(GetById), new { id = designElement.DesignElementId }, designElement);
        //}
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] DesignElementCreateDTO designElementDto)
        {
            string? imagePath = null;
            if (designElementDto.Image != null)
            {
                // Kiểm tra và thiết lập đường dẫn wwwroot
                string? webRootPath = _webHostEnvironment.WebRootPath;
                if (string.IsNullOrEmpty(webRootPath))
                {
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    Directory.CreateDirectory(webRootPath);
                }

                // Đảm bảo thư mục uploads tồn tại
                string uploadsFolder = Path.Combine(webRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                // Tạo tên file duy nhất
                string uniqueFileName = $"{Guid.NewGuid()}_{designElementDto.Image.FileName}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await designElementDto.Image.CopyToAsync(fileStream);
                }

                imagePath = $"/uploads/{uniqueFileName}"; // Lưu đường dẫn ảnh
            }

            // Chuyển đổi DTO thành Entity
            var designElement = _mapper.Map<DesignElement>(designElementDto);
            designElement.Image = imagePath; // Gán đường dẫn ảnh

            await _designElementService.AddDesignElement(designElement);
            return CreatedAtAction(nameof(GetById), new { id = designElement.DesignElementId }, designElement);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] DesignElement designElement)
        {
            if (id != designElement.DesignElementId)
                return BadRequest();
            await _designElementService.UpdateDesignElement(designElement);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _designElementService.DeleteDesignElement(id);
            return NoContent();
        }
    }

}
