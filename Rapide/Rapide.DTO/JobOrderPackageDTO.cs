namespace Rapide.DTO
{
    public class JobOrderPackageDTO : BaseDTO
    {
        public JobOrderDTO JobOrder { get; set; }
        public int JobOrderId { get; set; }

        public PackageDTO Package { get; set; }
        public int PackageId { get; set; }

        public decimal IncentiveSA { get; set; }

        public decimal IncentiveTech { get; set; }
    }
}