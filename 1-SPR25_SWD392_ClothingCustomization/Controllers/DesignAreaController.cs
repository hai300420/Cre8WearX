using _2_Service.Service;
using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;

namespace _1_SPR25_SWD392_ClothingCustomization.Controllers
{
    [Route("api/designareas")]
    [ApiController]
    public class DesignAreaController : Controller
    {
        private readonly IDesignAreaService _designAreaService;
        public DesignAreaController(IDesignAreaService designAreaService)
        {
            _designAreaService = designAreaService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DesignArea>>> GetAll()
        {
            return Ok(await _designAreaService.GetAllDesignAreas());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<DesignArea>> GetById(int id)
        {
            var designArea = await _designAreaService.GetDesignAreaById(id);
            if (designArea == null)
                return NotFound();
            return Ok(designArea);
        }
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] DesignArea designArea)
        {
            await _designAreaService.AddDesignArea(designArea);
            return CreatedAtAction(nameof(GetById), new { id = designArea.DesignAreaId }, designArea);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] DesignArea designArea)
        {
            if (id != designArea.DesignAreaId)
                return BadRequest();
            await _designAreaService.UpdateDesignArea(designArea);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _designAreaService.DeleteDesignArea(id);
            return NoContent();
        }
    }

}
