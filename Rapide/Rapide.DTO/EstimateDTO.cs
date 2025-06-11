namespace Rapide.DTO
{
    public class EstimateDTO : BaseDTO
    {
        public bool IsPackage { get; set; }

        public bool IsCustomerApproved { get; set; }

        public int? Inspectionid { get; set; }

        public string? ReferenceNo { get; set; }

        public DateTime? TransactionDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int EstimatedDays { get; set; }

        public JobStatusDTO JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public CustomerDTO Customer { get; set; }
        public int CustomerId { get; set; }

        public VehicleDTO Vehicle { get; set; }
        public int VehicleId { get; set; }

        public UserDTO AdvisorUser { get; set; }
        public int AdvisorUserId { get; set; }

        public UserDTO EstimatorUser { get; set; }
        public int EstimatorUserId { get; set; }

        public UserDTO ApproverUser { get; set; }
        public int ApproverUserId { get; set; }

        public ServiceGroupDTO ServiceGroup { get; set; }
        public int ServiceGroupId { get; set; }

        public int Odometer { get; set; }
        public int NextOdometerReminder { get; set; }

        public string? CustomerPO { get; set; }

        public string? Summary { get; set; }

        public decimal SubTotal { get; set; }

        public decimal VAT12 { get; set; }

        public decimal LaborDiscount { get; set; }

        public decimal ProductDiscount { get; set; }

        public decimal AdditionalDiscount { get; set; }

        public decimal TotalAmount { get; set; }

        public virtual List<EstimateServiceDTO> ServiceList { get; set; }
        public virtual List<EstimateProductDTO> ProductList { get; set; }
        public virtual List<EstimatePackageDTO> PackageList { get; set; }
        public virtual List<EstimateTechnicianDTO> TechnicianList { get; set; }
    }
}