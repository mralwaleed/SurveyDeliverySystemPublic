namespace SurveyDeliverySystem.Models
{
    public class SurveyResponse
    {

        public int TotalEmails { get; set; }
        public int SuccessfulEmails { get; set; }
        public int FailedEmails { get; set; }
        public string Message { get; set; }
    }
}
