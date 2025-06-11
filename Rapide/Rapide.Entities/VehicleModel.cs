using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class VehicleModel : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [ForeignKey("VehicleMakeId")]
        public VehicleMake VehicleMake { get; set; }
        public int VehicleMakeId { get; set; }

        [Required]
        [ForeignKey("BodyParameterId")]
        public Parameter BodyParameter { get; set; }
        public int BodyParameterId { get; set; }

        [Required]
        [ForeignKey("ClassificationParameterId")]
        public Parameter ClassificationParameter { get; set; }
        public int ClassificationParameterId { get; set; }
    }
}
