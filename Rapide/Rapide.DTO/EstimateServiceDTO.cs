namespace Rapide.DTO
{
    public class EstimateServiceDTO : BaseDTO
    {
        public bool IsPackage { get; set; }
        public int? PackageId { get; set; }

        public bool IsRequired { get; set; }

        public EstimateDTO Estimate { get; set; }
        public int EstimateId { get; set; }

        public ServiceDTO Service { get; set; }
        public int ServiceId { get; set; }

        public decimal Rate { get; set; }

        public decimal Hours { get; set; }

        public decimal Amount { get; set; }
    }
}