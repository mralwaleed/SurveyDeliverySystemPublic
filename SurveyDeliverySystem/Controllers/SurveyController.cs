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
            var response = new SurveyResponse
            {
                Message = "Survey emails are being processed."
            };

            var emailTasks = request.Domains
                .Select(domain => _surveyService.ProcessEmailAsync(new SurveyEmailInfo
                {
                    AdminEmail = domain.AdminEmail,
                    DomainName = domain.DomainName,
                    SurveyUrl = request.SurveyUrl
                }, response))
                .ToList();

            await Task.WhenAll(emailTasks);

            response.Message = $"Processed {response.TotalEmails} emails with {response.SuccessfulEmails} successes and {response.FailedEmails} failures.";

            return Ok(response);
        }
    }
}
