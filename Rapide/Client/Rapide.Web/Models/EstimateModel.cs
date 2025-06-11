using MudBlazor;

namespace Rapide.Web.Models
{
    public class EstimateModel
    {
        public int Id { get; set; }

        public bool IsCustomerApproved { get; set; }

        public int? Inspectionid { get; set; }

        public bool IsPackage { get; set; }

        public string? ReferenceNo { get; set; }

        public DateTime? TransactionDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int EstimatedDays { get; set; }

        public JobStatusModel JobStatus { get; set; }
        public int JobStatusId { get; set; }

        public CustomerModel Customer { get; set; }
        public int CustomerId { get; set; }

        public VehiclesModel Vehicle { get; set; }
        public int VehicleId { get; set; }

        public UserModel AdvisorUser { get; set; }
        public int AdvisorUserId { get; set; }

        public UserModel EstimatorUser { get; set; }
        public int EstimatorUserId { get; set; }

        public UserModel ApproverUser { get; set; }
        public int ApproverUserId { get; set; }

        public ServiceGroupModel ServiceGroup { get; set; }
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

        // Additional property
        public bool IsAllowedToOverride { get; set; }
        public Color StatusChipColor { get; set; }

    }
}
