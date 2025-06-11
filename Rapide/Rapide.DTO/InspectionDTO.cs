namespace Rapide.DTO
{
    public class InspectionDTO : BaseDTO
    {
        public string? ReferenceNo { get; set; }

        public DateTime? TransactionDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public JobStatusDTO JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public CustomerDTO Customer { get; set; }
        public int CustomerId { get; set; }

        public VehicleDTO Vehicle { get; set; }
        public int VehicleId { get; set; }

        public UserDTO InspectorUser { get; set; }
        public int InspectorUserId { get; set; }


        public ServiceGroupDTO ServiceGroup { get; set; }
        public int ServiceGroupId { get; set; }

        public UserDTO AdvisorUser { get; set; }
        public int AdvisorUserId { get; set; }

        public UserDTO EstimatorUser { get; set; }
        public int EstimatorUserId { get; set; }

        public UserDTO ApproverUser { get; set; }
        public int ApproverUserId { get; set; }

        public int Odometer { get; set; }

        public string? VehicleFindings { get; set; }

        public string InspectionDetails { get; set; }

        public string? Remarks { get; set; }

        public string? DiagnosticResult { get; set; }

        public virtual List<InspectionTechnicianDTO> TechnicianList { get; set; }
    }
}