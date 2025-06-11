using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class Service : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Code { get; set; }

        [Required]
        [ForeignKey("ServiceGroupId")]
        public ServiceGroup ServiceGroup { get; set; }
        public int ServiceGroupId { get; set; }

        [Required]
        [ForeignKey("ServiceCategoryId")]
        public ServiceCategory ServiceCategory { get; set; }
        public int ServiceCategoryId { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal StandardRate { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal StandardHours { get; set; }

        [Required]
        public bool IsReplacement { get; set; }

        [Required]
        public bool IsAllowRateOverride { get; set; }

        [Required]
        public bool IsMechanicRequired { get; set; }

        [Required]
        public bool DisplayStandardHours { get; set; }

        [Required]
        public bool DisplayStandardRate { get; set; }

        [Required]
        public bool DisplayNotes { get; set; }
    }
}
