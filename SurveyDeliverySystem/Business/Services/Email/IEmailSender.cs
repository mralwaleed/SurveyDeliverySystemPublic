using SurveyDeliverySystem.Models;

namespace SurveyDeliverySystem.Business.Services.Email
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(SurveyEmailInfo emailInfo);
    }
}
