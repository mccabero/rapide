namespace Rapide.DTO
{
    public class PettyCashDTO : BaseDTO
    {
        public string PCNo { get; set; }

        public DateTime? TransactionDateTime { get; set; }

        public string PayTo { get; set; }

        public decimal CashIn { get; set; }

        public decimal CashOut { get; set; }

        public decimal Balance { get; set; }

        public UserDTO ApprovedByUser { get; set; }
        public int ApprovedByUserId { get; set; }

        public UserDTO PaidByUser { get; set; }
        public int PaidByUserId { get; set; }

        public bool IsPaymentReceived { get; set; }

        public string? PaymentReceivedBy { get; set; }

        public JobStatusDTO JobStatus { get; set; }
        public int JobStatusId { get; set; }
    }
}