using System.ComponentModel.DataAnnotations;

namespace Rapide.DTO
{
    public class DepositDTO : BaseDTO
    {
        public string ReferenceNo { get; set; }

        public JobStatusDTO JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public DateTime? TransactionDateTime { get; set; }

        public CustomerDTO Customer { get; set; }
        public int CustomerId { get; set; }

        public JobOrderDTO JobOrder { get; set; }
        public int JobOrderId { get; set; }

        public ParameterDTO PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        public decimal DepositAmount { get; set; }

        public string? PaymentReferenceNo { get; set; }
        public string? Description { get; set; }

        // Refund part
        public bool IsRefund { get; set; }

        public decimal RefundAmount { get; set; }

        public DateTime? RefundDateTime { get; set; }

        public string? RefundReason { get; set; }

        public virtual UserDTO PreparedBy { get; set; }
    }
}