using System.ComponentModel.DataAnnotations;

namespace Rapide.Entities
{
    public class Package : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(50)]
        public string? Code { get; set; }

        [Required]
        public int NextServiceReminderDays { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal IncentiveSA { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal IncentiveTech { get; set; }

        public bool IsHideAmount { get; set; }

        public bool IsHideService { get; set; }

        public bool IsHidePartsAndMaterials { get; set; }

        public bool IsDisplayCode { get; set; }

        public string? Summary { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal VAT12 { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
    }
}