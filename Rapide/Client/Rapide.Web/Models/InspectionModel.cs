using MudBlazor;

namespace Rapide.Web.Models
{
    public class InspectionModel
    {
        public int Id { get; set; }

        public string? ReferenceNo { get; set; }

        public DateTime? TransactionDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public JobStatusModel JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public CustomerModel Customer { get; set; }
        public int CustomerId { get; set; }

        public VehiclesModel Vehicle { get; set; }
        public int VehicleId { get; set; }

        public UserModel InspectorUser { get; set; }
        public int InspectorUserId { get; set; }



        public ServiceGroupModel ServiceGroup { get; set; }
        public int ServiceGroupId { get; set; }

        public UserModel AdvisorUser { get; set; }
        public int AdvisorUserId { get; set; }

        public UserModel EstimatorUser { get; set; }
        public int EstimatorUserId { get; set; }

        public UserModel ApproverUser { get; set; }
        public int ApproverUserId { get; set; }

        public int Odometer { get; set; }

        public string? VehicleFindings { get; set; }

        public string InspectionDetails { get; set; }

        public string? Remarks { get; set; }

        public string? DiagnosticResult { get; set; }

        public Color StatusChipColor { get; set; }
        public bool IsAllowedToOverride { get; set; }
    }
}
