using MudBlazor;

namespace Rapide.Web.Models
{
    public class QuickSalesModel
    {
        public int Id { get; set; }

        public string? ReferenceNo { get; set; }

        public DateTime? TransactionDate { get; set; }

        public JobStatusModel JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public CustomerModel Customer { get; set; }
        public int CustomerId { get; set; }

        public ParameterModel PaymentTypeParameter { get; set; }
        public int PaymentTypeParameterId { get; set; }

        public string? PaymentReferenceNo { get; set; }

        public UserModel SalesPersonUser { get; set; }
        public int SalesPersonUserId { get; set; }

        public string? Summary { get; set; }

        public decimal SubTotal { get; set; }

        public decimal VAT12 { get; set; }

        public decimal Discount { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Payment { get; set; }

        public decimal Change { get; set; }

        public virtual List<QuickSalesProductModel> QuickSalesProduct { get; set; }

        // Additional property
        public bool IsAllowedToOverride { get; set; }
        public Color StatusChipColor { get; set; }
    }
}
