using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class JobOrder : BaseEntity
    {
        [Required]
        public bool IsPackage { get; set; }
        
        [Required]
        public bool IsPaid { get; set; }

        [Required]
        [ForeignKey("EstimateId")]
        public Estimate Estimate { get; set; }
        public int EstimateId { get; set; }

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

        [Required]
        [ForeignKey("ServiceGroupId")]
        public ServiceGroup ServiceGroup { get; set; }
        public int ServiceGroupId { get; set; }

        public int? Odometer { get; set; }
        public int? NextOdometerReminder { get; set; }

        public string? CustomerPO { get; set; }

        public string? Summary { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal VAT12 { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal LaborDiscount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal ProductDiscount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal AdditionalDiscount { get; set; }

        [Required]
        [DataType("decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
    }
}
