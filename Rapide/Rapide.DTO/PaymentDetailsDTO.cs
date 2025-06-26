namespace Rapide.DTO
{
    public class PaymentDetailsDTO : BaseDTO
    {
        public PaymentDTO Payment { get; set; }
        public int PaymentId { get; set; }

        public ParameterDTO PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        public InvoiceDTO Invoice { get; set; }
        public int InvoiceId { get; set; }

        public bool IsFullyPaid { get; set; }

        public DateTime? PaymentDate { get; set; }
        public bool IsDeposit { get; set; }
        
        //public decimal DepositAmount { get; set; }

        public decimal AmountPaid { get; set; }

        public string? PaymentReferenceNo { get; set; }
    }
}