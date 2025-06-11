using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class InvoicePackage : BaseEntity
    {
        [Required]
        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }
        public int InvoiceId { get; set; }

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