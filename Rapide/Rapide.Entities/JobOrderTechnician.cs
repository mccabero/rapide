using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class JobOrderTechnician : BaseEntity
    {
        [Required]
        [ForeignKey("JobOrderId")]
        public JobOrder JobOrder { get; set; }
        public int JobOrderId { get; set; }

        [Required]
        [ForeignKey("TechnicianUserId")]
        public User TechnicianUser { get; set; }
        public int TechnicianUserId { get; set; }
    }
}
