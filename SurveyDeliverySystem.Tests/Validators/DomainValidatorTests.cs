using NUnit.Framework;
using FluentValidation.TestHelper;
using SurveyDeliverySystem.Business.Validation;
using SurveyDeliverySystem.Models;

namespace SurveyDeliverySystem.Tests.Validators
{
    public class DomainValidatorTests
    {
        private DomainValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new DomainValidator();
        }

        [Test]
        public void Should_Have_Error_When_AdminEmail_Is_Empty()
        {
            var domain = new SurveyRequest.Domain { AdminEmail = "", DomainName = "test.com" };

            var result = _validator.TestValidate(domain);

            result.ShouldHaveValidationErrorFor(x => x.AdminEmail)
                .WithErrorMessage("ERROR_INVALID_EMAIL");
        }

        [Test]
        public void Should_Have_Error_When_AdminEmail_Is_Invalid()
        {
            var domain = new SurveyRequest.Domain { AdminEmail = "invalid-email", DomainName = "test.com" };

            var result = _validator.TestValidate(domain);

            result.ShouldHaveValidationErrorFor(x => x.AdminEmail)
                .WithErrorMessage("ERROR_INVALID_EMAIL: Admin email 'invalid-email' is not a valid email address.");
        }

        [Test]
        public void Should_Have_Error_When_DomainName_Is_Invalid()
        {
            var domain = new SurveyRequest.Domain { AdminEmail = "admin@test.com", DomainName = "invalid_domain" };

            var result = _validator.TestValidate(domain);

            result.ShouldHaveValidationErrorFor(x => x.DomainName)
                .WithErrorMessage("ERROR_INVALID_DOMAIN: Domain name 'invalid_domain' is not valid.");
        }

        [Test]
        public void Should_Have_Error_When_Email_Domain_Does_Not_Match()
        {
            var domain = new SurveyRequest.Domain { AdminEmail = "admin@test.com", DomainName = "different.com" };

            var result = _validator.TestValidate(domain);

            result.ShouldHaveValidationErrorFor(x => x)
                .WithErrorMessage("ERROR_INVALID_DOMAIN: Admin email 'admin@test.com' does not match the domain 'different.com'.");
        }

        [Test]
        public void Should_Not_Have_Error_When_Domain_And_Email_Are_Valid()
        {
            var domain = new SurveyRequest.Domain { AdminEmail = "admin@example.com", DomainName = "example.com" };

            var result = _validator.TestValidate(domain);

            result.ShouldNotHaveValidationErrorFor(x => x.AdminEmail);
            result.ShouldNotHaveValidationErrorFor(x => x.DomainName);
        }
    }
}
