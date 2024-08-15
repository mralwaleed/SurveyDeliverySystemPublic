using SurveyDeliverySystem.Business.Services.Email;
using SurveyDeliverySystem.Business.Validation;
using SurveyDeliverySystem.Enums;
using SurveyDeliverySystem.Models;
using System.Net.Mail;

namespace SurveyDeliverySystem.Business.Services.Survey
{
    public class SurveyService : ISurveyService
    {
        private readonly IEmailSender _emailSender;
        private readonly Validator _validator;
        private readonly ILogger<SurveyService> _logger;

        public SurveyService(IEmailSender emailSender, Validator validator, ILogger<SurveyService> logger)
        {
            _emailSender = emailSender;
            _validator = validator;
            _logger = logger;
        }

        public async Task ProcessEmailAsync(SurveyEmailInfo emailInfo, SurveyResponse response)
        {
            bool isValid = true;

            if (!_validator.Validate(emailInfo.AdminEmail, ValidationType.Email))
            {
                response.FailedEmails++;
                _logger.LogWarning($"Invalid email address: {emailInfo.AdminEmail}");
                isValid = false;
            }

            if (!_validator.Validate(emailInfo.SurveyUrl, ValidationType.Url))
            {
                response.FailedEmails++;
                _logger.LogWarning($"Invalid survey URL: {emailInfo.SurveyUrl}");
                isValid = false;
            }

            if (!_validator.Validate(emailInfo.DomainName, ValidationType.Domain))
            {
                response.FailedEmails++;
                _logger.LogWarning($"Invalid domain name: {emailInfo.DomainName}");
                isValid = false;
            }

            // Proceed with email sending only if all validations pass
            if (isValid)
            {
                bool success = await _emailSender.SendEmailAsync(emailInfo);
                if (success)
                {
                    response.SuccessfulEmails++;
                }
                else
                {
                    response.FailedEmails++;
                    _logger.LogError($"Failed to send email to {emailInfo.AdminEmail} for domain {emailInfo.DomainName}");
                }
            }
            else
            {
                _logger.LogError("Email sending aborted due to validation errors.");
            }
        }


    }
}
