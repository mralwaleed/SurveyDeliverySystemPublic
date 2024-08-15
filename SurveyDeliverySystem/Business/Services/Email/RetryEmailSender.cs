using Microsoft.Extensions.Options;
using SurveyDeliverySystem.Config;
using SurveyDeliverySystem.Models;
using System.Net;
using System.Net.Mail;

namespace SurveyDeliverySystem.Business.Services.Email
{
    public class RetryEmailSender : IEmailSender
    {
        private readonly IEmailSender _innerEmailSender;
        private readonly ILogger<RetryEmailSender> _logger;
        private readonly EmailSettingsConfig _emailSettings;

        public RetryEmailSender(IEmailSender innerEmailSender, IOptions<EmailSettingsConfig> emailSettings, ILogger<RetryEmailSender> logger)
        {
            _innerEmailSender = innerEmailSender;
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendEmailAsync(SurveyEmailInfo emailInfo)
        {
            int attempt = 0;
            while (attempt < _emailSettings.MaxRetryAttempts)
            {
                try
                {
                    if (await _innerEmailSender.SendEmailAsync(emailInfo))
                    {
                        _logger.LogInformation($"Email sent successfully to {emailInfo.AdminEmail} for domain {emailInfo.DomainName} on attempt {attempt + 1}.");
                        return true;
                    }
                }
                catch (SmtpException ex) when (IsTransient(ex))
                {
                    attempt++;
                    _logger.LogWarning($"Transient SMTP exception occurred. Retrying {attempt}/{_emailSettings.MaxRetryAttempts}...");
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Exponential backoff
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unexpected error occurred while sending email to {emailInfo.AdminEmail} for domain {emailInfo.DomainName}. Attempt {attempt + 1}/{_emailSettings.MaxRetryAttempts}.");
                    return false;
                }
            }
            _logger.LogError($"Failed to send email to {emailInfo.AdminEmail} for domain {emailInfo.DomainName} after {attempt} attempts.");
            return false;
        }

        private bool IsTransient(SmtpException ex)
        {
            return ex.StatusCode == SmtpStatusCode.ServiceNotAvailable ||
                   ex.StatusCode == SmtpStatusCode.TransactionFailed ||
                   ex.InnerException is WebException webEx && webEx.Status == WebExceptionStatus.Timeout;
        }
    }
}
