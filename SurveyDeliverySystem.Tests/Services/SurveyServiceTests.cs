using NUnit.Framework;
using Moq;
using SurveyDeliverySystem.Business.Services.Email;
using SurveyDeliverySystem.Business.Services.Survey;
using SurveyDeliverySystem.Models;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace SurveyDeliverySystem.Tests.Services
{
    public class SurveyServiceTests
    {
        private Mock<IEmailSender> _emailSenderMock;
        private Mock<IValidator<SurveyRequest>> _validatorMock;
        private SurveyService _surveyService;

        [SetUp]
        public void Setup()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _validatorMock = new Mock<IValidator<SurveyRequest>>();
            var loggerMock = Mock.Of<ILogger<SurveyService>>();

            _surveyService = new SurveyService(_emailSenderMock.Object, _validatorMock.Object, loggerMock);
        }

        [Test]
        public async Task Should_Not_Send_Emails_When_Validation_Fails()
        {
            var request = new SurveyRequest { SurveyUrl = "invalid-url" };
            _validatorMock.Setup(v => v.Validate(request)).Returns(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("SurveyUrl", "Invalid URL")
            }));

            var response = new SurveyResponse();

            await _surveyService.ProcessEmailAsync(request, response);

            _emailSenderMock.Verify(x => x.SendEmailsInBccAsync(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
            Assert.AreEqual("Validation errors occurred. No emails were sent.", response.Message);
        }

        [Test]
        public async Task Should_Send_Emails_When_Validation_Succeeds()
        {
            var request = new SurveyRequest
            {
                SurveyUrl = "https://valid-url.com",
                Domains = new List<SurveyRequest.Domain>
            {
                new SurveyRequest.Domain { AdminEmail = "admin@example.com", DomainName = "example.com" }
            }
            };

            _validatorMock.Setup(v => v.Validate(request)).Returns(new ValidationResult());

            _emailSenderMock.Setup(x => x.SendEmailsInBccAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ReturnsAsync(true);

            var response = new SurveyResponse();

            await _surveyService.ProcessEmailAsync(request, response);

            _emailSenderMock.Verify(x => x.SendEmailsInBccAsync(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Once);
            Assert.AreEqual("Successfully sent emails to 1 recipients.", response.Message);
        }
    }
}
