using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class EstimateTechnician : BaseEntity
    {
        [Required]
        [ForeignKey("EstimateId")]
        public Estimate Estimate { get; set; }
        public int EstimateId { get; set; }

        [Required]
        [ForeignKey("TechnicianUserId")]
        public User TechnicianUser { get; set; }
        public int TechnicianUserId { get; set; }
    }
}
