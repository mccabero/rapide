namespace Rapide.DTO
{
    public class UserRolesDTO : BaseDTO
    {
        public UserDTO User { get; set; }
        public int UserId { get; set; }

        public RoleDTO Role { get; set; }
        public int RoleId { get; set; }
    }
}