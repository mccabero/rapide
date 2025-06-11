namespace Rapide.Web.Models
{
    public class JobOrderProductModel
    {
        public int Id { get; set; }

        public bool IsPackage { get; set; }
        public int? PackageId { get; set; }

        public bool IsRequired { get; set; }

        public JobOrderModel JobOrder { get; set; }
        public int JobOrderId { get; set; }

        public ProductModel Product { get; set; }
        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public int Qty { get; set; }

        public decimal Amount { get; set; }

        public decimal IncentiveSA { get; set; }

        public decimal IncentiveTech { get; set; }
    }
}
