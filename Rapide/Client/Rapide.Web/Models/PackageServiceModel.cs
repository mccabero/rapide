namespace Rapide.Web.Models
{
    public class PackageServiceModel
    {
        public int Id { get; set; }

        public PackageModel? Package { get; set; }
        public int PackageId { get; set; }

        public ServiceModel? Service { get; set; }
        public int ServiceId { get; set; }

        public decimal? Rate { get; set; }

        public decimal? Hours { get; set; }

        public decimal? Amount { get; set; }

        public int ctr { get; set; }
    }
}
