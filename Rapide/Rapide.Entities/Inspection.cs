using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class Inspection : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string? ReferenceNo { get; set; }

        public DateTime? TransactionDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [Required]
        [ForeignKey("JobStatusId")]
        public JobStatus JobStatus { get; set; }
        public int JobStatusId { get; set; }

        [Required]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

        [Required]
        [ForeignKey("VehicleId")]
        public Vehicle Vehicle { get; set; }
        public int VehicleId { get; set; }

        [Required]
        [ForeignKey("InspectorUserId")]
        public User InspectorUser { get; set; }
        public int InspectorUserId { get; set; }



        [Required]
        [ForeignKey("ServiceGroupId")]
        public ServiceGroup ServiceGroup { get; set; }
        public int ServiceGroupId { get; set; }

        [Required]
        [ForeignKey("AdvisorUserId")]
        public User AdvisorUser { get; set; }
        public int AdvisorUserId { get; set; }

        [Required]
        [ForeignKey("EstimatorUserId")]
        public User EstimatorUser { get; set; }
        public int EstimatorUserId { get; set; }

        [Required]
        [ForeignKey("ApproverUserId")]
        public User ApproverUser { get; set; }
        public int ApproverUserId { get; set; }


        public int? Odometer { get; set; }

        public string? VehicleFindings { get; set; }

        [Required]
        public string InspectionDetails { get; set; }

        public string? Remarks { get; set; }

        public string? DiagnosticResult { get; set; }
    }
}
