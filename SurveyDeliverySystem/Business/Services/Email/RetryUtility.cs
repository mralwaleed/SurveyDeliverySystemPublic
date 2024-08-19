using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using SurveyDeliverySystem.Config;

namespace SurveyDeliverySystem.Business.Services.Email
{
    public class RetryUtility : IRetryUtility
    {
        private readonly int _maxRetryAttempts;
        private readonly int _retryBaseDelaySeconds;
        private readonly ILogger<RetryUtility> _logger;
        public RetryUtility(IOptions<EmailSettingsConfig> emailSettings, ILogger<RetryUtility> logger)
        {
            _maxRetryAttempts = emailSettings.Value.MaxRetryAttempts;
            _retryBaseDelaySeconds = emailSettings.Value.RetryBaseDelaySeconds;
            _logger = logger;
        }
        public async Task<bool> ExecuteAsync(Func<Task<bool>> action)
        {
            int attempt = 0;
            while (attempt < _maxRetryAttempts)
            {
                try
                {
                    if (await action())
                    {
                        return true; // If the action is successful, return true
                    }
                }
                catch (SmtpException ex) when (IsTransient(ex))
                {
                    attempt++;
                    _logger.LogWarning($"Transient SMTP exception occurred. Retrying {attempt}/{_maxRetryAttempts}...");
                    await Task.Delay(TimeSpan.FromSeconds(_retryBaseDelaySeconds * Math.Pow(2, attempt - 1))); // Exponential backoff
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unexpected error occurred during attempt {attempt + 1}/{_maxRetryAttempts}.");
                    return false;
                }
            }
            _logger.LogError($"Failed after {attempt} attempts.");
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
