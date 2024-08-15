using SurveyDeliverySystem.Models;

namespace SurveyDeliverySystem.Business.Services.Survey
{
    public interface ISurveyService
    {
        Task ProcessEmailAsync(SurveyEmailInfo emailInfo, SurveyResponse response);
    }
}
