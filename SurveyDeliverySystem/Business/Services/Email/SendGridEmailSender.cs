using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SurveyDeliverySystem.Config;
using SurveyDeliverySystem.Models;
using System.Net.Mail;
using static SurveyDeliverySystem.Models.SurveyRequest;


namespace SurveyDeliverySystem.Business.Services.Email
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridSettingsConfig _sendGridConfig;
        private readonly ILogger<SendGridEmailSender> _logger;
        private readonly IRetryUtility _retryUtility;



        public SendGridEmailSender(IOptions<SendGridSettingsConfig> sendGridConfig, IRetryUtility retryUtility, ILogger<SendGridEmailSender> logger)
        {
            _sendGridConfig = sendGridConfig.Value;
            _logger = logger;
            _retryUtility = retryUtility;

        }





        public async Task<bool> SendEmailsInBccAsync(string surveyUrl, List<string> bccEmails)
        {
            return await _retryUtility.ExecuteAsync(async () =>
            {
                try
                {
                    var client = new SendGridClient(_sendGridConfig.ApiKey);
                    var from = new EmailAddress(_sendGridConfig.FromEmail, _sendGridConfig.FromName);
                    var subject = "Survey Invitation";
                    var to = new EmailAddress("noreply@alwaleedAPP.com"); // Dummy TO address, since all recipients will be in BCC.
                    var plainTextContent = $"Please take the survey at {surveyUrl}.";
                    var htmlContent = $"<strong>Please take the survey at {surveyUrl}.</strong>";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                    foreach (var bccEmail in bccEmails)
                    {
                        msg.AddBcc(new EmailAddress(bccEmail));
                    }

                    var response = await client.SendEmailAsync(msg);

                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        _logger.LogInformation($"Email sent successfully to {string.Join(", ", bccEmails)}.");
                        return true;
                    }
                    else
                    {
                        _logger.LogError($"Failed to send email. Status code: {response.StatusCode}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to send email: {ex.Message}");
                    return false;
                }
            });
        }
    }
}
