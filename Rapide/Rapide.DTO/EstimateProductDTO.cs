namespace Rapide.DTO
{
    public class EstimateProductDTO : BaseDTO
    {
        public bool IsPackage { get; set; }
        public int? PackageId { get; set; }

        public bool IsRequired { get; set; }


        public EstimateDTO Estimate { get; set; }
        public int EstimateId { get; set; }

        public ProductDTO Product { get; set; }
        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public int Qty { get; set; }

        public decimal Amount { get; set; }

        public decimal IncentiveSA { get; set; }

        public decimal IncentiveTech { get; set; }
    }
}