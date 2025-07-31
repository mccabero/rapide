using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rapide.Web.Helpers
{
    public static class TokenHelper
    {
        public static bool IsRoleEqual(AuthenticationState authState, string role)
        {
            var claims = authState.User.Claims;
            var roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (roles == null)
                return false;

            var userRoles = JsonConvert.DeserializeObject<List<string>>(roles.Value);

            if (userRoles.Any(x => x.Equals(role, StringComparison.InvariantCultureIgnoreCase)))
                return true;
            else
                return false;
        }

        public static bool RoleIsInList(AuthenticationState authState, List<string> roleList)
        {
            var claims = authState.User.Claims;
            var roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (roles == null)
                return false;

            var userRoles = JsonConvert.DeserializeObject<List<string>>(roles.Value);

            if (userRoles.Intersect(roleList).Any())
                return true;
            else
                return false;
        }

        public static bool IsBigThreeRoles(AuthenticationState authState)
        {
            var claims = authState.User.Claims;
            var roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (roles == null)
                return false;

            var userRoles = JsonConvert.DeserializeObject<List<string>>(roles.Value);

            if (userRoles.Any(x => x.Equals(Constants.UserRoles.Owner))) 
                return true;
            if (userRoles.Any(x => x.Equals(Constants.UserRoles.Supervisor)))
                return true;
            if (userRoles.Any(x => x.Equals(Constants.UserRoles.SystemAdministrator)))
                return true;
            if (userRoles.Any(x => x.Equals(Constants.UserRoles.RapideAdministrator)))
                return true;

            return false;
        }

        public static bool IsBigThreeRolesWithoutSupervisor(AuthenticationState authState)
        {
            var claims = authState.User.Claims;
            var roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (roles == null)
                return false;

            var userRoles = JsonConvert.DeserializeObject<List<string>>(roles.Value);

            if (userRoles.Any(x => x.Equals(Constants.UserRoles.Owner)))
                return true;
            if (userRoles.Any(x => x.Equals(Constants.UserRoles.SystemAdministrator)))
                return true;
            if (userRoles.Any(x => x.Equals(Constants.UserRoles.RapideAdministrator)))
                return true;

            return false;
        }

        public static int GetCurrentUserId(AuthenticationState authState)
        {
            var claims = authState.User.Claims;

            if (claims.Any())
            {
                var id = claims.Where(x => x.Type == "Id");

                return id.Any()
                    ? int.Parse(id.FirstOrDefault().Value)
                    : 0;
            }

            return 0;
        }

        public static List<string> GetUserRoles(AuthenticationState authState)
        {
            var claims = authState.User.Claims;
            var roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (roles == null)
                return new List<string>();

            var userRoles = JsonConvert.DeserializeObject<List<string>>(roles.Value);

            return userRoles;
        }

        public static bool IsTokenIsValid(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return true;
            }

            var jwtToken = new JwtSecurityToken(token);
            return (jwtToken == null) || (jwtToken.ValidTo > DateTime.Now);
        }

        public static async Task<LoginResponseModel> GenerateClaimsResponse(UserDTO user)
        {
            try
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.SecretKey));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                List<Claim> claims = [];

                claims = GenerateUserClaims(user);

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: Constants.IssuerValue,
                    audience: Constants.AudienceValue,
                    claims: claims,
                    expires: DateTime.Now.AddDays(Constants.TokenExpiryValue),
                    
                    signingCredentials: signinCredentials
                );

                var response = new LoginResponseModel();

                response.IsAuthenticated = true;
                response.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                response.ErrorMessage = null;
                response.User = user;

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        private static List<Claim> GenerateUserClaims(UserDTO? user)
        {
            var userClaims = new List<Claim>()
                {
                    new Claim("Id", user.Id.ToString()  ?? string.Empty),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"  ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email.ToString()  ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.Role.Name.ToString())
                };

            if (user.UserRoles == null)
                return userClaims;

            foreach (var u in user.UserRoles)
                userClaims.Add(new Claim(ClaimTypes.Role, u.Role.Name));

            return userClaims;
        }
    }
}
