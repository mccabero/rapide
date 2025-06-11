using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class PackageService : BaseEntity
    {
        [Required]
        [ForeignKey("PackageId")]
        public Package Package { get; set; }
        public int PackageId { get; set; }

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
