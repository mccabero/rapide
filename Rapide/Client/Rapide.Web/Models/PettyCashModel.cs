using MudBlazor;
using Rapide.DTO;

namespace Rapide.Web.Models
{
    public class PettyCashModel
    {
        public int Id { get; set; }
        public string PCNo { get; set; }

        public DateTime? TransactionDateTime { get; set; }

        public string PayTo { get; set; }

        public string Particulars { get; set; }

        public decimal CashIn { get; set; }

        public decimal CashOut { get; set; }

        public decimal Balance { get; set; }

        public UserModel PaidByUser { get; set; }
        public int PaidByUserId { get; set; }

        public bool IsPaymentReceived { get; set; }

        public string? PaymentReceivedBy { get; set; }

        // Additional property
        public bool IsAllowedToOverride { get; set; }

        public bool IsChangan { get; set; }
    }
}