using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class UserRoles : BaseEntity
    {
        [Required]
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }

        [Required]
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
        public int RoleId { get; set; }
    }
}