using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class JobOrderService : BaseEntity
    {
        [Required]
        public bool IsPackage { get; set; }
        public int? PackageId { get; set; }

        [Required]
        public bool IsRequired { get; set; }

        [Required]
        [ForeignKey("JobOrderId")]
        public JobOrder JobOrder { get; set; }
        public int JobOrderId { get; set; }

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
