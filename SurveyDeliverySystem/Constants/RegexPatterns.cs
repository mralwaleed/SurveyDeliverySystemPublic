using System.Text.RegularExpressions;

namespace SurveyDeliverySystem.Constants
{
    public class RegexPatterns
    {
        public static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        public static readonly Regex UrlRegex = new Regex(@"^(https?:\/\/)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}(:\d{1,5})?(\/.*)?$", RegexOptions.Compiled);
        public static readonly Regex DomainRegex = new Regex(@"^(?!:\/\/)([a-zA-Z0-9-_]+\.)*[a-zA-Z0-9][a-zA-Z0-9-_]+\.[a-zA-Z]{2,6}?$", RegexOptions.Compiled);
        // this is static calsss for regex patterns used in the application for validation 
        // we use this calss to if you need update or add will be in one place 
        // https://regex101.com/ is a good website to test regex patterns 
    }
}
