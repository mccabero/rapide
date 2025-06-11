using System.ComponentModel.DataAnnotations;

namespace Rapide.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CreatedById { get; set; }

        [Required]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        public int UpdatedById { get; set; }

        [Required]
        public DateTime UpdatedDateTime { get; set; }
    }
}
