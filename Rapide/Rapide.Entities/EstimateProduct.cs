using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class EstimateProduct : BaseEntity
    {
        [Required]
        public bool IsPackage { get; set; }
        public int? PackageId { get; set; }

        [Required]
        public bool IsRequired { get; set; }

        [Required]
        [ForeignKey("EstimateId")]
        public Estimate Estimate { get; set; }
        public int EstimateId { get; set; }

        [Required]
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int ProductId { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Price { get; set; }

        public int Qty { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal IncentiveSA { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal IncentiveTech { get; set; }
    }
}
