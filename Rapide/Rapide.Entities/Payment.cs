using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class Payment : BaseEntity
    {
        [Required]
        public bool IsFullyPaid { get; set; }
        
        [Required]
        public string ReferenceNo { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [ForeignKey("JobStatusId")]
        public JobStatus JobStatus { get; set; }
        public int JobStatusId { get; set; }

        [Required]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal InvoiceTotalAmount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal VAT12 { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal DepositAmount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal AmountPayable { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal TotalPaidAmount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Balance { get; set; }

        public string? Remarks { get; set; }
    }
}