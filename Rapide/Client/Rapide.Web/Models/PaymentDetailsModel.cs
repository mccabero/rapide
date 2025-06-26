namespace Rapide.Web.Models
{
    public class PaymentDetailsModel
    {
        public int Id { get; set; }

        public PaymentModel Payment { get; set; }
        public int PaymentId { get; set; }

        public ParameterModel PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        public InvoiceModel Invoice { get; set; }
        public int InvoiceId { get; set; }

        public bool IsFullyPaid { get; set; }

        //public decimal DepositAmount { get; set; }

        public bool IsDeposit { get; set; }

        public DateTime? PaymentDate { get; set; }

        public decimal AmountPaid { get; set; }

        public string? PaymentReferenceNo { get; set; }
    }
}