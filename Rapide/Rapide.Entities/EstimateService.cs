using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class EstimateService : BaseEntity
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
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }
        public int ServiceId { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Rate { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Hours { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Amount { get; set; }
    }
}
