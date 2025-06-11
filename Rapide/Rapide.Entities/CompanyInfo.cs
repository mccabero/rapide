using System.ComponentModel.DataAnnotations;

namespace Rapide.Entities
{
    public class CompanyInfo : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(255)]
        public string? Address { get; set; }

        [Required]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(50)]
        public string? MobileNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string? TIN { get; set; }

        [Required]
        [StringLength(50)]
        public string? GCash { get; set; }

        [Required]
        [StringLength(100)]
        public string? BankNo { get; set; }
    }
}
