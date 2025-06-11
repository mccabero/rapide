using System.ComponentModel.DataAnnotations;

namespace Rapide.Entities
{
    public class ServiceCategory : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
