namespace Rapide.DTO
{
    public class PaymentDTO : BaseDTO
    {
        public bool IsFullyPaid { get; set; }

        public string? ReferenceNo { get; set; }

        public DateTime? PaymentDate { get; set; }

        public JobStatusDTO JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public CustomerDTO Customer { get; set; }
        public int CustomerId { get; set; }

        public decimal InvoiceTotalAmount { get; set; }

        public decimal VAT12 { get; set; }

        public decimal DepositAmount { get; set; }

        public decimal AmountPayable { get; set; }

        public decimal TotalPaidAmount { get; set; }

        public decimal Balance { get; set; }

        public string? Remarks { get; set; }

        public virtual List<InvoiceDTO> InvoiceList { get; set; }
        public virtual List<PaymentDetailsDTO> PaymentDetailsList { get; set; }
    }
}