using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.Services;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Rapide.Web.Components.Pages
{
    public partial class Forgot
    {
        [Inject]
        public AccountService _accountService { get; set; }
        [Inject]
        public ILocalStorageService _localStorageService { get; set; }
        [Inject]
        public IUserService UserService { get; set; }

        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }

        private ClaimsPrincipal User { get; set; }

        private ForgotPasswordRequestModel ForgotPasswordRequestModel { get; set; } = new();
        private ForgotPasswordResponseModel ForgotPasswordResponseModel { get; set; } = new();
        private string ErrorMessage = string.Empty;
        private MudTextField<string> pwField1;
        private MudMessageBox mbox { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        protected async override Task OnInitializedAsync()
        {
            User = await GetUserPrincipal();

            if (User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateToCustom("/");
            }

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            User = await GetUserPrincipal();

            if (User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateToCustom("/");
            }

            if (!string.IsNullOrEmpty(ForgotPasswordResponseModel.ErrorMessage))
                ErrorMessage = ForgotPasswordResponseModel.ErrorMessage;

            StateHasChanged();
            await base.OnAfterRenderAsync(firstRender);
        }

        private string PasswordMatch(string arg)
        {
            if (pwField1.Value != arg)
                return "Passwords don't match";
            return null;
        }

        private IEnumerable<string> PasswordStrength(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw))
            {
                yield return "Password is required!";
                yield break;
            }
            if (pw.Length < 8)
                yield return "Password must be at least of length 8";
            if (!Regex.IsMatch(pw, @"[A-Z]"))
                yield return "Password must contain at least one capital letter";
            if (!Regex.IsMatch(pw, @"[a-z]"))
                yield return "Password must contain at least one lowercase letter";
            if (!Regex.IsMatch(pw, @"[0-9]"))
                yield return "Password must contain at least one digit";
        }

        private async Task ForgotPasswordUser()
        {
            await form.Validate();
            if (!form.IsValid)
                return;

            var userList = await UserService.GetAllUserRoleAsync();
            var supervisorUser = userList.Where(x => x.Role.Name.Equals(Constants.UserRoles.Supervisor)).ToList();

            var currentUserInfo = userList.Where(x => x.Email.Equals(ForgotPasswordRequestModel.Email)).ToList();
            if (currentUserInfo == null || !currentUserInfo.Any())
            {
                ErrorMessage = $"user with email {ForgotPasswordRequestModel.Email} not found!";
                NavigationManager.NavigateTo("/forgot-password", true);
            }

            var supervisorPasswordHash = CryptographyHelper.Encrypt(ForgotPasswordRequestModel.SupervisorPassword, CryptographyHelper.GetEncryptionKey());
            var supervisorInfo = userList.Where(x => x.Email == ForgotPasswordRequestModel.SupervisorEmail);

            if (supervisorInfo == null || !supervisorInfo.Any())
            {
                ErrorMessage = "Supervisor account not found!";
                NavigationManager.NavigateTo("/forgot-password", true);
                return;
            }

            var supervisorRole = supervisorInfo.Where(x => x.Role.Name.Equals(Constants.UserRoles.Supervisor)).ToList();
            if (supervisorRole == null || !supervisorRole.Any())
            {
                ErrorMessage = "Supervisor account not found!";
                NavigationManager.NavigateTo("/forgot-password", true);
                return;
            }


            if (!supervisorRole.FirstOrDefault().PasswordHash.Equals(supervisorPasswordHash))
            {
                ErrorMessage = "Invalid supervisor credential!";
                NavigationManager.NavigateTo("/forgot-password", true);
                return;
            }

            var newUserPassword = CryptographyHelper.Encrypt(ForgotPasswordRequestModel.ConfirmPassword, CryptographyHelper.GetEncryptionKey());
            var userToUpdate = currentUserInfo.FirstOrDefault();

            userToUpdate.PasswordHash = newUserPassword;

            await UserService.UpdateAsync(userToUpdate);
            NavigationManager.NavigateTo("/login", false);

        }

        async Task<ClaimsPrincipal> GetUserPrincipal()
        {
            return (await AuthState).User;
        }
    }
}
