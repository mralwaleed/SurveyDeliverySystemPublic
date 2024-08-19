using NUnit.Framework;
using FluentValidation.TestHelper;
using SurveyDeliverySystem.Business.Validation;
using SurveyDeliverySystem.Models;

namespace SurveyDeliverySystem.Tests.Validators
{
    public class SurveyRequestValidatorTests
    {
        private SurveyRequestValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new SurveyRequestValidator();
        }

        [Test]
        public void Should_Have_Error_When_SurveyUrl_Is_Empty()
        {
            var request = new SurveyRequest { SurveyUrl = "" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.SurveyUrl)
                .WithErrorMessage("ERROR_INVALID_URL: Survey URL is required.");
        }

        [Test]
        public void Should_Have_Error_When_SurveyUrl_Is_Invalid()
        {
            var request = new SurveyRequest { SurveyUrl = "invalid-url" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.SurveyUrl)
                .WithErrorMessage("ERROR_INVALID_URL: Survey URL 'invalid-url' is not a valid absolute URL.");
        }

        [Test]
        public void Should_Not_Have_Error_When_SurveyUrl_Is_Valid()
        {
            var request = new SurveyRequest { SurveyUrl = "https://valid-url.com" };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(x => x.SurveyUrl);
        }
    }
}
