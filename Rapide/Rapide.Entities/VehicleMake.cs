using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class VehicleMake : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [ForeignKey("RegionParameterId")]
        public Parameter RegionParameter { get; set; }
        public int RegionParameterId { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
