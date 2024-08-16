using FluentValidation;
using SurveyDeliverySystem.Business.Services.Email;
using SurveyDeliverySystem.Business.Validation;
using SurveyDeliverySystem.Enums;
using SurveyDeliverySystem.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace SurveyDeliverySystem.Business.Services.Survey
{
    public class SurveyService : ISurveyService
    {
        private readonly IEmailSender _emailSender;
        private readonly IValidator<SurveyEmailInfo> _validator;
        private readonly ILogger<SurveyService> _logger;

        public SurveyService(IEmailSender emailSender, IValidator<SurveyEmailInfo> validator, ILogger<SurveyService> logger)
        {
            _emailSender = emailSender;
            _validator = validator;
            _logger = logger;
        }

        public async Task ProcessEmailAsync(SurveyEmailInfo emailInfo, SurveyResponse response)
        {
            var validationResult = _validator.Validate(emailInfo);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                }

                response.FailedEmails++;
                return;
            }

            // Proceed with email sending if validation passes
            bool success = await _emailSender.SendEmailAsync(emailInfo);
            if (success)
            {
                response.SuccessfulEmails++;
            }
            else
            {
                response.FailedEmails++;
            }
        }
    }
}
