using Rapide.DTO;

namespace Rapide.Web.Models
{
    public class EstimateServiceModel
    {
        public int Id { get; set; }

        public bool IsPackage { get; set; }

        public bool IsRequired { get; set; }

        public EstimateModel Estimate { get; set; }
        public int EstimateId { get; set; }

        public ServiceModel Service { get; set; }
        public int ServiceId { get; set; }

        public decimal? Rate { get; set; }

        public decimal? Hours { get; set; }

        public decimal? Amount { get; set; }
    }
}
