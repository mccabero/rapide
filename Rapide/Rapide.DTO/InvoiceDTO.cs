namespace Rapide.DTO
{
    public class InvoiceDTO : BaseDTO
    {
        public bool IsPackage { get; set; }

        public string? InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? DueDate { get; set; }

        public JobOrderDTO JobOrder { get; set; }
        public int JobOrderId { get; set; }

        public JobStatusDTO JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public CustomerDTO Customer { get; set; }
        public int CustomerId { get; set; }

        public string? CustomerPO { get; set; }

        public UserDTO AdvisorUser { get; set; }
        public int AdvisorUserId { get; set; }

        public string? Summary { get; set; }

        public decimal SubTotal { get; set; }

        public decimal VAT12 { get; set; }

        public decimal LaborDiscount { get; set; }

        public decimal ProductDiscount { get; set; }

        public decimal AdditionalDiscount { get; set; }

        public decimal TotalAmount { get; set; }

        public virtual List<JobOrderServiceDTO> ServiceList { get; set; }
        public virtual List<JobOrderProductDTO> ProductList { get; set; }
        public virtual List<JobOrderTechnicianDTO> TechnicianList { get; set; }
        public virtual List<InvoicePackageDTO> PackageList { get; set; }

        public virtual List<PaymentDetailsDTO> PaymentDetailsList { get; set; }
    }
}