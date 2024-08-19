using SurveyDeliverySystem.Models;

namespace SurveyDeliverySystem.Business.Services.Survey
{
    public interface ISurveyService
    {
        Task ProcessEmailAsync(SurveyRequest emailInfo, SurveyResponse response);
    }
}
