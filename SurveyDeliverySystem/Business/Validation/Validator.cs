using Microsoft.Extensions.Logging;
using SurveyDeliverySystem.Business.Services.Email;
using SurveyDeliverySystem.Constants;
using SurveyDeliverySystem.Enums;
using System.Text.RegularExpressions;

namespace SurveyDeliverySystem.Business.Validation
{
    public class Validator
    {
        private readonly ILogger<Validator> _logger;

        public Validator(ILogger<Validator> logger)
        {
            _logger = logger;
        }
        public bool Validate(string value, ValidationType validationType)
        {
            if (string.IsNullOrEmpty(value))
            {
                _logger.LogInformation($"Validation failed: value is null or empty for {validationType}.");
                return false;
            }

            bool isValid = validationType switch
            {
                ValidationType.Email => RegexPatterns.EmailRegex.IsMatch(value),
                ValidationType.Url => RegexPatterns.UrlRegex.IsMatch(value),
                ValidationType.Domain => RegexPatterns.DomainRegex.IsMatch(value),
                _ => throw new ArgumentException("Invalid validation type"),
            };

            _logger.LogInformation($"Validating {validationType}: '{value}' - Valid: {isValid}");
            return isValid;
        }

    }
}
