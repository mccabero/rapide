using MudBlazor;

namespace Rapide.Web.Models
{
    public class ExpensesModel
    {
        public bool IsPaid { get; set; }

        public int Id { get; set; }

        public string ReferenceNo { get; set; }

        public DateTime? ExpenseDateTime { get; set; }

        public decimal Amount { get; set; }

        public decimal VAT12 { get; set; }

        public string PayTo { get; set; }

        public string? Remarks { get; set; }

        public string PaymentReferenceNo { get; set; }

        public ParameterModel PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        public JobStatusModel JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public UserModel ExpenseByUser { get; set; }
        public int ExpenseByUserId { get; set; }

        public Color StatusChipColor { get; set; }

        // Additional property
        public bool IsAllowedToOverride { get; set; }
    }
}