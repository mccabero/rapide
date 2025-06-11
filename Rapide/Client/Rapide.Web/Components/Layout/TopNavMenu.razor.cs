using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Helpers;
using Rapide.Web.Services;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Rapide.Web.Components.Layout
{
    public partial class TopNavMenu
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        public AccountService _accountService { get; set; }
        [Inject]
        private IUserRolesService UserRolesService { get; set; }
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        ClaimsPrincipal UserClaims { get; set; } = new();
        UserDTO CurrentUser { get; set; } = new();
        private string FullName { get; set; }
        private string PrimaryRole { get; set; }
        private int RoleCount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var id = TokenHelper.GetCurrentUserId(await AuthState);
            CurrentUser = await _accountService.GetCurrentLoggedInUser(id);
            var userRoles = await UserRolesService.GetUserRolesByUserIdAsync(id);

            if (CurrentUser != null)
            {
                FullName = $"{CurrentUser.FirstName} {CurrentUser.LastName}";
                PrimaryRole = CurrentUser.Role.Name;
                RoleCount = userRoles.Count();
            }
                

            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            

            await base.OnAfterRenderAsync(firstRender);
        }
        public async Task OnLogoutClick()
        {
            await _accountService.Logout();
            NavigationManager.NavigateToCustom("/login", true);
        }

        private async Task<ClaimsPrincipal> GetUserPrincipal()
        {
            if (UserClaims == null)
            {
                UserClaims = (await AuthState).User;
            }

            return UserClaims;
        }
    }
}
