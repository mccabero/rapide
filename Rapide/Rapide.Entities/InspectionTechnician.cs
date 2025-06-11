using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class InspectionTechnician : BaseEntity
    {
        [Required]
        [ForeignKey("InspectionId")]
        public Inspection Inspection { get; set; }
        public int InspectionId { get; set; }

        [Required]
        [ForeignKey("TechnicianUserId")]
        public User TechnicianUser { get; set; }
        public int TechnicianUserId { get; set; }
    }
}
