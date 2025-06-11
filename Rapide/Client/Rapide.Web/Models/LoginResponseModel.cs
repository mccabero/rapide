using Rapide.DTO;

namespace Rapide.Web.Models
{
    public class LoginResponseModel
    {
        public bool IsAuthenticated { get; set; }

        public string? Token { get; set; }

        public string? ErrorMessage { get; set; }

        public UserDTO User { get; set; }
    }
}
