using MudBlazor;
using Rapide.DTO;

namespace Rapide.Web.Models
{
    public class DepositModel
    {
        public int Id { get; set; }

        public string ReferenceNo { get; set; }

        public JobStatusModel JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public DateTime? TransactionDateTime { get; set; }

        public CustomerModel Customer { get; set; }
        public int CustomerId { get; set; }

        public JobOrderModel JobOrder { get; set; }
        public int JobOrderId { get; set; }

        public ParameterModel PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        public decimal DepositAmount { get; set; }

        public string? PaymentReferenceNo { get; set; }
        public string? Description { get; set; }

        public Color StatusChipColor { get; set; }

        // Refund part
        public bool IsRefund { get; set; }

        public decimal RefundAmount { get; set; }

        public DateTime? RefundDateTime { get; set; }

        public string? RefundReason { get; set; }

        public UserDTO PreparedBy { get; set; }

        // Additional property
        public bool IsAllowedToOverride { get; set; }
    }
}