using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class Deposit : BaseEntity
    {
        [Required]
        public string ReferenceNo { get; set; }

        [Required]
        [ForeignKey("JobStatusId")]
        public JobStatus JobStatus { get; set; }
        public int JobStatusId { get; set; }

        [Required]
        public DateTime TransactionDateTime { get; set; }

        [Required]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

        [Required]
        [ForeignKey("JobOrderId")]
        public JobOrder JobOrder { get; set; }
        public int JobOrderId { get; set; }

        [Required]
        [ForeignKey("PaymentTypeParameterId")]
        public Parameter PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal DepositAmount { get; set; }

        public string? PaymentReferenceNo { get; set; }
        public string? Description { get; set; }

        // Refund part
        public bool IsRefund { get; set; }

        [DataType("decimal(18, 2)")]
        public decimal RefundAmount { get; set; }

        public DateTime? RefundDateTime { get; set; }

        public string? RefundReason { get; set; }
    }
}