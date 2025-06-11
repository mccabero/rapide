namespace Rapide.Web.Models
{
    public class UserRolesModel
    {
        public UserModel User { get; set; }
        public int UserId { get; set; }

        public RoleModel Role { get; set; }
        public int RoleId { get; set; }
    }
}
