namespace Rapide.DTO
{
    public class EstimatePackageDTO : BaseDTO
    {
        public EstimateDTO Estimate { get; set; }
        public int EstimateId { get; set; }

        public PackageDTO Package { get; set; }
        public int PackageId { get; set; }

        public decimal IncentiveSA { get; set; }

        public decimal IncentiveTech { get; set; }
    }
}