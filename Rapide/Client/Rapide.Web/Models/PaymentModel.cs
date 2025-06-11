using MudBlazor;

namespace Rapide.Web.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }

        public bool IsFullyPaid { get; set; }

        public string? ReferenceNo { get; set; }

        public DateTime? PaymentDate { get; set; }

        public JobStatusModel JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public CustomerModel Customer { get; set; }
        public int CustomerId { get; set; }

        public decimal InvoiceTotalAmount { get; set; }

        public decimal VAT12 { get; set; }

        public decimal DepositAmount { get; set; }

        public decimal AmountPayable { get; set; }

        public decimal TotalPaidAmount { get; set; }

        public decimal Balance { get; set; }

        public string? Remarks { get; set; }

        public virtual List<InvoiceModel> InvoiceList { get; set; }

        public virtual List<PaymentDetailsModel> PaymentDetailsList { get; set; }

        // Additional property
        public bool IsAllowedToOverride { get; set; }
        public Color StatusChipColor { get; set; }
    }
}