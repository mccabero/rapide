using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class PaymentDetails : BaseEntity
    {
        [Required]
        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }
        public int PaymentId { get; set; }

        [Required]
        [ForeignKey("PaymentTypeParameterId")]
        public Parameter PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        [Required]
        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }
        public int InvoiceId { get; set; }

        [Required]
        public bool IsFullyPaid { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal DepositAmount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal AmountPaid { get; set; }

        public string? PaymentReferenceNo { get; set; }
    }
}