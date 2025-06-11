namespace Rapide.Web.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        public string? FullName 
        {
            get { return $"{FirstName} {MiddleName} {LastName}"; }
            set { }
        }

        public string? Email { get; set; }

        public string? MobileNumber { get; set; }

        public int Gender { get; set; }

        public DateTime Birthday { get; set; }

        public bool IsActive { get; set; }

        public RoleModel Role { get; set; }
        public int RoleId { get; set; }
    }
}
