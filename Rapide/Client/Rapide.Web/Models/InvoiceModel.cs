using MudBlazor;
using Rapide.DTO;

namespace Rapide.Web.Models
{
    public class InvoiceModel
    {
        public int Id { get; set; }

        public bool IsPackage { get; set; }

        public string? InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? DueDate { get; set; }

        public JobOrderModel JobOrder { get; set; }
        public int JobOrderId { get; set; }

        public JobStatusModel JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public CustomerModel Customer { get; set; }
        public int CustomerId { get; set; }

        public string? CustomerPO { get; set; }

        public UserModel AdvisorUser { get; set; }
        public int AdvisorUserId { get; set; }

        public string? Summary { get; set; }

        public decimal SubTotal { get; set; }

        public decimal VAT12 { get; set; }

        public decimal LaborDiscount { get; set; }

        public decimal ProductDiscount { get; set; }

        public decimal AdditionalDiscount { get; set; }

        public decimal TotalAmount { get; set; }

        public Color StatusChipColor { get; set; }

        public decimal DepositAmount { get; set; }
    }
}
