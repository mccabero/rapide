namespace Rapide.Web.Models
{
    public class EstimateProductModel
    {
        public int Id { get; set; }

        public bool IsPackage { get; set; }

        public bool IsRequired { get; set; }

        public EstimateModel Estimate { get; set; }
        public int EstimateId { get; set; }

        public ProductModel Product { get; set; }
        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public int Qty { get; set; }

        public decimal Amount { get; set; }

        public decimal IncentiveSA { get; set; }

        public decimal IncentiveTech { get; set; }
    }
}
