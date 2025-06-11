using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class EstimatePackage : BaseEntity
    {
        [Required]
        [ForeignKey("EstimateId")]
        public Estimate Estimate { get; set; }
        public int EstimateId { get; set; }

        [Required]
        [ForeignKey("PackageId")]
        public Package Package { get; set; }
        public int PackageId { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal IncentiveSA { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal IncentiveTech { get; set; }
    }
}