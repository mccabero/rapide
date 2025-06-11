using System.ComponentModel.DataAnnotations;

namespace Rapide.Entities
{
    public class Manufacturer : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
