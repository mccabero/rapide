using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class Expenses : BaseEntity
    {
        [Required]
        public bool IsPaid { get; set; }

        [Required]
        public string ReferenceNo { get; set; }

        public DateTime? ExpenseDateTime { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal VAT12 { get; set; }

        [Required]
        public string PayTo { get; set; }

        public string? Remarks { get; set; }

        public string? PaymentReferenceNo { get; set; }

        [Required]
        [ForeignKey("PaymentTypeParameterId")]
        public Parameter PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        [Required]
        [ForeignKey("JobStatusId")]
        public JobStatus JobStatus { get; set; }
        public int JobStatusId { get; set; }

        [Required]
        [ForeignKey("ExpenseByUserId")]
        public User ExpenseByUser { get; set; }
        public int ExpenseByUserId { get; set; }
    }
}