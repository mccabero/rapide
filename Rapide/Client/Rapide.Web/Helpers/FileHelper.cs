namespace Rapide.Web.Helpers
{
    public static class FileHelper
    {
        public static byte[] GetRapideLogo()
        {
            var image = new FileInfo("wwwroot/images/default-logo/default-logo.png");
            byte[] bytes = File.ReadAllBytes(image.FullName);

            return bytes;
        }

        public static byte[] GetCompanyLogo()
        {
            var image = new FileInfo("wwwroot/images/company-logo/company-logo.jpg");
            byte[] bytes = File.ReadAllBytes(image.FullName);

            return bytes;
        }

        public static string GetInspectionTemplate()
        {
            var text = File.ReadAllText("wwwroot/inspection-template.json");
            
            return text;

        }
    }
}
