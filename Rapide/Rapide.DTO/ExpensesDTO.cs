namespace Rapide.DTO
{
    public class ExpensesDTO : BaseDTO
    {
        public bool IsPaid { get; set; }

        public string ReferenceNo { get; set; }

        public DateTime? ExpenseDateTime { get; set; }

        public decimal Amount { get; set; }

        public decimal VAT12 { get; set; }

        public string PayTo { get; set; }

        public string? Remarks { get; set; }

        public string? PaymentReferenceNo { get; set; }

        public ParameterDTO PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        public JobStatusDTO JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public UserDTO ExpenseByUser { get; set; }
        public int ExpenseByUserId { get; set; }
    }
}