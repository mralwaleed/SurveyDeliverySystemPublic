namespace SurveyDeliverySystem.Business.Services
{
    public interface IRetryUtility
    {
        Task<bool> ExecuteAsync(Func<Task<bool>> action);

    }
}
