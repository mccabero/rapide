using System.ComponentModel.DataAnnotations;

namespace Rapide.Entities
{
    public class Supplier : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [Required]
        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [Required]
        [StringLength(50)]
        public string? ContactNumber { get; set; }
    }
}
