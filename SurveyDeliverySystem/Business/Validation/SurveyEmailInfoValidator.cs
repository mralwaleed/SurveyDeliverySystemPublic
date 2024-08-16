using FluentValidation;
using SurveyDeliverySystem.Constants;
using SurveyDeliverySystem.Models;

namespace SurveyDeliverySystem.Business.Validation
{
    public class SurveyEmailInfoValidator : AbstractValidator<SurveyEmailInfo>
    {
        public SurveyEmailInfoValidator()
        {
            // https://regex101.com/ is a good website to test regex patterns 

            RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage(ErrorKeys.InvalidEmail)
            .EmailAddress().WithMessage(ErrorKeys.InvalidEmail);

            RuleFor(x => x.DomainName)
                .NotEmpty().WithMessage(ErrorKeys.InvalidDomain)
                .Matches(@"^(?!:\/\/)([a-zA-Z0-9-_]+\.)*[a-zA-Z0-9][a-zA-Z0-9-_]+\.[a-zA-Z]{2,6}?$")
                .WithMessage(ErrorKeys.InvalidDomain);

            RuleFor(x => x.SurveyUrl)
                .NotEmpty().WithMessage(ErrorKeys.InvalidUrl)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage(ErrorKeys.InvalidUrl);

            // Custom rule to validate that the AdminEmail domain matches the specified DomainName
            RuleFor(x => x)
                .Must(MatchEmailDomain)
                .WithMessage(ErrorKeys.InvalidDomain);

        }


        
        private bool MatchEmailDomain(SurveyEmailInfo emailInfo)
        {
            if (string.IsNullOrEmpty(emailInfo.AdminEmail) || string.IsNullOrEmpty(emailInfo.DomainName))
            {
                return false;
            }

 
            var emailDomain = emailInfo.AdminEmail.Split('@').Last();
            return string.Equals(emailDomain, emailInfo.DomainName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
