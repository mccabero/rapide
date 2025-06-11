using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class QuickSales : BaseEntity
    {
        [Required]
        public string ReferenceNo { get; set; }

        [Required]
        public DateTime? TransactionDate { get; set; }

        [Required]
        [ForeignKey("JobStatusId")]
        public JobStatus JobStatus { get; set; }
        public int JobStatusId { get; set; }

        [Required]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

        [Required]
        [ForeignKey("PaymentTypeParameterId")]
        public Parameter PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        public string? PaymentReferenceNo { get; set; }

        [Required]
        [ForeignKey("SalesPersonUserId")]
        public User SalesPersonUser { get; set; }
        public int SalesPersonUserId { get; set; }

        public string? Summary { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal VAT12 { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Discount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Payment { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Change { get; set; }
    }
}