using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class JobOrderPackage : BaseEntity
    {
        [Required]
        [ForeignKey("JobOrderId")]
        public JobOrder JobOrder { get; set; }
        public int JobOrderId { get; set; }

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