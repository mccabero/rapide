namespace Rapide.DTO
{
    public class InvoicePackageDTO : BaseDTO
    {
        public InvoiceDTO Invoice { get; set; }
        public int InvoiceId { get; set; }

        public PackageDTO Package { get; set; }
        public int PackageId { get; set; }

        public decimal IncentiveSA { get; set; }

        public decimal IncentiveTech { get; set; }
    }
}