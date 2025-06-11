using System.ComponentModel.DataAnnotations;

namespace Rapide.Entities
{
    public class Customer : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }

        [Required]
        public int Gender { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(50)]
        public string? CustomerCode { get; set; }

        [StringLength(50)]
        public string? MobileNumber { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? HomeAddress { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        [StringLength(255)]
        public string? CompanyName { get; set; }

        [StringLength(255)]
        public string? CompanyAddress { get; set; }

        [StringLength(50)]
        public string? CompanyNo { get; set; }

        public bool IsActive { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal LaborDiscountRate { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal ProductDiscountRate { get; set; }

        public bool IsVATExempt { get; set; }

        public bool IsAllowWithholidingTax { get; set; }
    }
}
