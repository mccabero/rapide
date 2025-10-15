using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class PettyCash : BaseEntity
    {
        [Required]
        public string PCNo { get; set; }

        public DateTime? TransactionDateTime { get; set; }

        public string Particulars { get; set; }

        [Required]
        public string PayTo { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal CashIn { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal CashOut { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal Balance { get; set; }

        [Required]
        [ForeignKey("PaidByUserId")]
        public User PaidByUser { get; set; }
        public int PaidByUserId { get; set; }

        public bool IsPaymentReceived { get; set; }

        [Required]
        public string PaymentReceivedBy { get; set; }
    }
}