using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity.Data;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class OverrideTransactionComponent
    {
        
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IUserRolesService UserRolesService { get; set; }

        private LoginRequestModel LoginRequestModel { get; set; } = new();
        private LoginResponseModel LoginResponseModel { get; set; } = new();
        private string ErrorMessage = string.Empty;
        private bool IsSupervisorValid = false;

        protected override async Task OnInitializedAsync()
        {
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task OnValidateClick()
        {
            IsSupervisorValid = true;

            // Login flow here...
            var userByEmail = await UserService.GetUserRoleByEmailAsync(LoginRequestModel.Email);

            if (userByEmail == null)
            {
                ErrorMessage = "User with email not found!";
                IsSupervisorValid = false;

                return;
            }

            // Check if password is match
            var encryptedPassword = CryptographyHelper.Encrypt(LoginRequestModel.Password, CryptographyHelper.GetEncryptionKey());

            if (userByEmail.PasswordHash != encryptedPassword)
            {
                ErrorMessage = "Password is invalid!";
                IsSupervisorValid = false;

                return;
            }

            // list of roles
            userByEmail.UserRoles = await UserRolesService.GetUserRolesByUserIdAsync(userByEmail.Id);

            if (userByEmail.UserRoles == null || !userByEmail.UserRoles.Any())
            {
                ErrorMessage = "No user roles found for this user. Please contact your supervisor or systems administrator!";
                IsSupervisorValid = false;

                return;
            }

            if (!userByEmail.UserRoles.Where(x => x.Role.Name.Equals(Constants.UserRoles.Supervisor)).Any()
                && !userByEmail.UserRoles.Where(x => x.Role.Name.Equals(Constants.UserRoles.OIC)).Any())
            {
                ErrorMessage = "Current user does not have a valid OIC or Supervisor role!";
                IsSupervisorValid = false;

                return;
            }

            MudDialog.Close(DialogResult.Ok(IsSupervisorValid));
        }
    }
}
