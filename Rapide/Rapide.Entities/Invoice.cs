using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class Invoice : BaseEntity
    {
        [Required]
        public bool IsPackage { get; set; }

        [Required]
        [StringLength(255)]
        public string? InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        [ForeignKey("JobOrderId")]
        public JobOrder JobOrder { get; set; }
        public int JobOrderId { get; set; }

        [Required]
        [ForeignKey("JobStatusId")]
        public JobStatus JobStatus { get; set; }
        public int JobStatusId { get; set; }

        [Required]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

        public string? CustomerPO { get; set; }

        [Required]
        [ForeignKey("AdvisorUserId")]
        public User AdvisorUser { get; set; }
        public int AdvisorUserId { get; set; }

        public string? Summary { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal VAT12 { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal LaborDiscount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal ProductDiscount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal AdditionalDiscount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
    }
}
