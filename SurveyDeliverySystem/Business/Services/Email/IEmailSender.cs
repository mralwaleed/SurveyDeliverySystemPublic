using SurveyDeliverySystem.Models;

namespace SurveyDeliverySystem.Business.Services.Email
{
    public interface IEmailSender
    {
        Task<bool> SendEmailsInBccAsync(string surveyUrl, List<string> bccEmails);
    }
}
