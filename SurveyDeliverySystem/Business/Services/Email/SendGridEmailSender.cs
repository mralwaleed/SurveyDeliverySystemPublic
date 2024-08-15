using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SurveyDeliverySystem.Config;
using SurveyDeliverySystem.Models;
using System.Net.Mail;

namespace SurveyDeliverySystem.Business.Services.Email
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridSettingsConfig _sendGridConfig;
        private readonly ILogger<RetryEmailSender> _logger;

        public SendGridEmailSender(IOptions<SendGridSettingsConfig> sendGridConfig, ILogger<RetryEmailSender> logger)
        {
            _sendGridConfig = sendGridConfig.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(SurveyEmailInfo emailInfo)
        {


            // Simulate a failure on the first attempt to test retry logic :) 
            // change to 1 to 4 if you want to test the chance of failure and test the retry logic :)
            if (new Random().Next(4, 4) == 1)  //25% chance to simulate failure 
            {
                throw new SmtpException(SmtpStatusCode.ServiceNotAvailable, "Simulated transient failure");
            }

            var client = new SendGridClient(_sendGridConfig.ApiKey);
            var from = new EmailAddress(_sendGridConfig.FromEmail, _sendGridConfig.FromName);
            var subject = $"Survey for {emailInfo.DomainName}";
            var to = new EmailAddress(emailInfo.AdminEmail);
            var plainTextContent = $"Please take the survey at {emailInfo.SurveyUrl}.";
            var htmlContent = $"<strong>Please take the survey at {emailInfo.SurveyUrl}.</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            //  return response.StatusCode == System.Net.HttpStatusCode.OK;

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogInformation($"Email sent successfully to {emailInfo.AdminEmail} for domain {emailInfo.DomainName}.");
                return true;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger.LogError("Failed to send email: Forbidden. Check your API key and permissions.");
                return false;
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError($"Failed to send email to {emailInfo.AdminEmail} for domain {emailInfo.DomainName}. Status Code: {response.StatusCode}, Response: {responseBody}");
                return false;
            }
        }
    }
}
