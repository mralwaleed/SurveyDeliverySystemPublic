using SurveyDeliverySystem.Models;
using System.Net.Mail;

namespace SurveyDeliverySystem.Business.Services.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(SmtpClient smtpClient, ILogger<SmtpEmailSender> logger)
        {
            _smtpClient = smtpClient;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(SurveyEmailInfo emailInfo)
        {
            try
            {
                var mailMessage = new MailMessage("no-reply@alwaleed.com", emailInfo.AdminEmail)
                {
                    Subject = $"Survey for {emailInfo.DomainName}",
                    Body = $"Please take the survey at {emailInfo.SurveyUrl}.", // we can enhance the email body by use HTML tags to make it more attractive. 
                    IsBodyHtml = true
                }; // You can take parameters for the subject and body of the email from Database or configuration file it will be better if you will use more one time. 

                await _smtpClient.SendMailAsync(mailMessage); // Here we use SendMailAsync method to send email asynchronously. 
                _logger.LogInformation($"Email sent successfully to {emailInfo.AdminEmail} for domain {emailInfo.DomainName}.");
                return true;
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError($"SMTP error when sending email to {emailInfo.AdminEmail} for domain {emailInfo.DomainName}: {smtpEx.Message}"); // Always we try to log the error message to trace the error :) .
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error when sending email to {emailInfo.AdminEmail} for domain {emailInfo.DomainName}: {ex.Message}");
                return false;
            }
        }
    }
}
