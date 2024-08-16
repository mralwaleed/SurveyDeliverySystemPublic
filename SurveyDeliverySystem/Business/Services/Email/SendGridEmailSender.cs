using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SurveyDeliverySystem.Config;
using SurveyDeliverySystem.Models;
using System.Net.Mail;
using Polly; 

namespace SurveyDeliverySystem.Business.Services.Email
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridSettingsConfig _sendGridConfig;
        private readonly ILogger<SendGridEmailSender> _logger;
        private readonly AsyncPolicy<bool> _resiliencePolicy;



        public SendGridEmailSender(IOptions<SendGridSettingsConfig> sendGridConfig, IOptions<EmailSettingsConfig> emailSettingsConfig, ILogger<SendGridEmailSender> logger)
        {
            _sendGridConfig = sendGridConfig.Value;
            _logger = logger;



            var emileSetting = emailSettingsConfig.Value;


            var retryPolicy = Policy.Handle<Exception>() // or use SmtpException 
                .WaitAndRetryAsync(
                retryCount: emileSetting.MaxRetryAttempts,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(emileSetting.RetryBaseDelaySeconds * Math.Pow(2, attempt - 1)), // Exponential backoff
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning($"Failed to send email. Retrying attempt {retryCount} after {timeSpan}. Exception: {exception.Message}");
                });

            // Define a fallback policy
                    var fallbackPolicy = Policy<bool>
            .Handle<SmtpException>()
            .Or<Exception>()
            .FallbackAsync(
                fallbackValue: false, // Fallback value in case of failure
                onFallbackAsync: (exception, context) =>
                {
                    _logger.LogError($"All retries failed. Executing fallback due to: {exception.Exception.Message}");
                    return Task.CompletedTask;
                }
            );


            // Combine the retry and fallback policies
            _resiliencePolicy = fallbackPolicy.WrapAsync(retryPolicy);



        }





        public async Task<bool> SendEmailAsync(SurveyEmailInfo emailInfo)
        {

            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                // Simulate a failure on the first attempt to test retry logic :) 
                // change to 1 to 4 if you want to test the chance of failure and test the retry logic :)
                if (new Random().Next(1, 1) == 1)  // 25% chance to simulate failure
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
                 throw new SmtpException(SmtpStatusCode.ServiceNotAvailable, "Simulated transient failure");
                }
                else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError($"Failed to send email to {emailInfo.AdminEmail} for domain {emailInfo.DomainName}. Status Code: {response.StatusCode}, Response: {responseBody}");
                return false;
            }
            });
        }
    }
}
