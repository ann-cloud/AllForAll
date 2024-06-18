

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllForAll.Models;
using BusinessLogic.Dto.Feedback;
using BusinessLogic.Interfaces;
using BusinessLogic.Implementation;

namespace AllForAll.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IUserService _userService;


        public FeedbackController(IFeedbackService feedbackService, IUserService userService)
        {
            _feedbackService = feedbackService;
            _userService = userService;


        }

        // GET: api/Feedback
        [HttpGet]
        public async Task<IActionResult> GetFeedbacks(CancellationToken cancellationToken)
        {
            var feedbacks = await _feedbackService.GetAllFeedbacksAsync(cancellationToken);
            return Ok(feedbacks);
        }

        // GET: api/Feedback/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedback([FromRoute] int id, CancellationToken cancellationToken)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id, cancellationToken);
            if (feedback == null)
            {
                return NotFound();
            }
            return Ok(feedback);
        }

        // POST: api/Feedback
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostFeedback([FromBody] FeedbackRequestDto feedbackDto, CancellationToken cancellationToken)
        {

            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { Message = "Invalid token format or token missing" });
            }
            string token = authorizationHeader.Substring("Bearer ".Length);
            if (string.IsNullOrEmpty(token) || token == "null")
            {
                return BadRequest(new { Message = "The token does not exist or is null" });
            }
            var user = await _userService.CheckToken(token);
            feedbackDto.UserId = user.UserId;
            var feedbackId = await _feedbackService.CreateFeedbackAsync(feedbackDto, cancellationToken);

            return Ok(feedbackId);
        }

        // PUT: api/Feedback
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeedback([FromRoute] int id, [FromBody] FeedbackRequestDto feedbackDto, CancellationToken cancellation = default)
        {
            await _feedbackService.UpdateFeedbackAsync(id, feedbackDto, cancellation);
            return NoContent();
        }

        // DELETE: api/Feedback/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback([FromRoute] int id, CancellationToken cancellation = default)
        {
            await _feedbackService.DeleteFeedbackAsync(id, cancellation);
            return NoContent();
        }

    }
}
