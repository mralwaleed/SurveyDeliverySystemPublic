using FluentValidation;
using SurveyDeliverySystem.Constants;
using SurveyDeliverySystem.Models;

namespace SurveyDeliverySystem.Business.Validation
{
    public class DomainValidator : AbstractValidator<SurveyRequest.Domain>
    {
        public DomainValidator()
        {

            RuleFor(x => x.AdminEmail)
                 .NotEmpty().WithMessage(ErrorKeys.InvalidEmail)
                 .EmailAddress().WithMessage(domain => $"{ErrorKeys.InvalidEmail}: Admin email '{domain.AdminEmail}' is not a valid email address.");

            RuleFor(x => x.DomainName)
                .NotEmpty().WithMessage(ErrorKeys.InvalidDomain)
                .Matches(RegexPatterns.DomainRegex)
                .WithMessage(domain => $"{ErrorKeys.InvalidDomain}: Domain name '{domain.DomainName}' is not valid.");

            // Custom rule to validate that the AdminEmail domain matches the specified DomainName
            RuleFor(x => x)
                .Must(MatchEmailDomain)
                .WithMessage(domain => $"{ErrorKeys.InvalidDomain}: Admin email '{domain.AdminEmail}' does not match the domain '{domain.DomainName}'.");

        }

        private bool MatchEmailDomain(SurveyRequest.Domain domain)
        {
            if (string.IsNullOrEmpty(domain.AdminEmail) || string.IsNullOrEmpty(domain.DomainName))
            {
                return false;
            }

            // Extract the domain from the AdminEmail
            var emailDomain = domain.AdminEmail.Split('@').Last();
            return string.Equals(emailDomain, domain.DomainName, StringComparison.OrdinalIgnoreCase);
        }

    }
}
