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
        private readonly IValidator<SurveyRequest> _validator;
        private readonly ILogger<SurveyService> _logger;

        public SurveyService(IEmailSender emailSender, IValidator<SurveyRequest> validator, ILogger<SurveyService> logger)
        {
            _emailSender = emailSender;
            _validator = validator;
            _logger = logger;
        }

        public async Task ProcessEmailAsync(SurveyRequest request, SurveyResponse response)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                    response.Errors.ValidationErrors += $"{error.ErrorMessage}\n";
                }
                response.Message = "Validation errors occurred. No emails were sent.";
                return;
            }
            var emailAddresses = request.Domains.Select(domain => domain.AdminEmail).ToList();
            response.TotalEmails = emailAddresses.Count;

            // Proceed with sending emails in BCC if all are valid
            response.TotalEmails = emailAddresses.Count;

            if (emailAddresses.Any())
            {
                bool success = await _emailSender.SendEmailsInBccAsync(request.SurveyUrl, emailAddresses);

                response.Message = success
                    ? $"Successfully sent emails to {response.TotalEmails} recipients."
                    : "Failed to send emails.";

                if (!success)
                {
                    response.Errors.ProcessingErrors = "Failed to send emails due to an unknown error.";
                }
            }
            else
            {
                response.Message = "No valid email addresses found. No emails were sent.";
            }


        }
    }
}
