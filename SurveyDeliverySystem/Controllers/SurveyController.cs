using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyDeliverySystem.Business.Services.Survey;
using SurveyDeliverySystem.Models;

namespace SurveyDeliverySystem.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]


    public class SurveyController : ControllerBase
    {
        private readonly ISurveyService _surveyService;
        private readonly ILogger<SurveyController> _logger;

        public SurveyController(ISurveyService surveyService, ILogger<SurveyController> logger)
        {
            _surveyService = surveyService;
            _logger = logger;
        }

        [HttpPost("create-survey")]
        public async Task<IActionResult> CreateSurvey([FromBody] SurveyRequest request)
        {
            var response = new SurveyResponse();

            // Process all emails and send them together in BCC
            await _surveyService.ProcessEmailAsync(request, response);

            return Ok(response);
        }
    }
}
