namespace SurveyDeliverySystem.Models
{
    public class SurveyRequest
    {
        public string SurveyUrl { get; set; }
        public List<Domain> Domains { get; set; }

        public class Domain
        {
            public string DomainName { get; set; }
            public string AdminEmail { get; set; }
        }
    }
}
