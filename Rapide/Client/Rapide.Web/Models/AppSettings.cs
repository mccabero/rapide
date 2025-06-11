namespace Rapide.Web.Models
{
    public class AppSettings
    {
        public string Secret { get; set; } = string.Empty;

        public int TokenExpirationInDays { get; set; }

        public string ApiUrl { get; set; } = string.Empty;

        public string ApiIssuer { get; set; } = string.Empty;
    }
}
