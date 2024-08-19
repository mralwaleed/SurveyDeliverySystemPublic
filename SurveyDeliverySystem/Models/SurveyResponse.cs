namespace SurveyDeliverySystem.Models
{
    public class SurveyResponse
    {
        public int TotalEmails { get; set; }
        public string Message { get; set; }
        public Errors Errors { get; set; } = new Errors(); 
        public bool IsSuccess => string.IsNullOrEmpty(Errors.ValidationErrors) && string.IsNullOrEmpty(Errors.ProcessingErrors) && TotalEmails > 0; 
    }

    public class Errors
    {
        public string ValidationErrors { get; set; } = string.Empty; 
        public string ProcessingErrors { get; set; } = string.Empty; 

        public bool HasErrors => !string.IsNullOrEmpty(ValidationErrors) || !string.IsNullOrEmpty(ProcessingErrors);
    }

}
