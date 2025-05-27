using _2_Service.Service;
using BusinessObject;
using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using static BusinessObject.RequestDTO.RequestDTO;

namespace _1_SPR25_SWD392_ClothingCustomization.Controllers
{
    [Route("api/Feedbacks")]
    [ApiController]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetAll()
        {
            return Ok(await _feedbackService.GetAllFeedbacks());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Feedback>> GetById(int id)
        {
            var feedback = await _feedbackService.GetFeedbackById(id);
            if (feedback == null)
                return NotFound();
            return Ok(feedback);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] FeedbackDTO feedbackDto)
        {
            var feedback = new Feedback
            {
                OrderId = feedbackDto.OrderId,
                UserId = feedbackDto.UserId,
                Rating = feedbackDto.Rating,
                Review = feedbackDto.Review,
                CreatedDate = DateTime.Now,
            };
            await _feedbackService.AddFeedback(feedback);
            return CreatedAtAction(nameof(GetById), new { id = feedback.FeedbackId }, feedback);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] FeedbackDTO feedbackDTO)
        {
            var existingFeedback = await _feedbackService.GetFeedbackById(id);
            if (existingFeedback == null)
            {
                return NotFound();
            }

            // Update only allowed properties
            existingFeedback.Review = feedbackDTO.Review;
            existingFeedback.Rating = feedbackDTO.Rating;

            await _feedbackService.UpdateFeedback(existingFeedback);
            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _feedbackService.DeleteFeedback(id);
            return NoContent();
        }
    }
}
