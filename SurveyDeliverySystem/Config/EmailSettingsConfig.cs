namespace SurveyDeliverySystem.Config
{
    public class EmailSettingsConfig
    {
        public int MaxRetryAttempts { get; set; } = 3; // Default value if not configured
        public int RetryBaseDelaySeconds { get; set; } = 2; // Default base delay for retries


    }
}
