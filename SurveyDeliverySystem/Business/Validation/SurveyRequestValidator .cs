using FluentValidation;
using SurveyDeliverySystem.Constants;
using SurveyDeliverySystem.Models;

namespace SurveyDeliverySystem.Business.Validation
{
    public class SurveyRequestValidator : AbstractValidator<SurveyRequest>
    {
        public SurveyRequestValidator()
        {
            RuleFor(x => x.SurveyUrl)
                .NotEmpty().WithMessage(ErrorKeys.InvalidUrl + ": Survey URL is required.")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage(x => $"{ErrorKeys.InvalidUrl}: Survey URL '{x.SurveyUrl}' is not a valid absolute URL.");


            RuleForEach(x => x.Domains)
                .SetValidator(new DomainValidator());
        }
    }
 }
