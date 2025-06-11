using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.Services;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Rapide.Web.Components.Pages
{
    public partial class Login
    {
        [Inject]
        public AccountService _accountService { get; set; }
        [Inject]
        public ILocalStorageService _localStorageService { get; set; }
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }

        private ClaimsPrincipal User { get; set; }

        private LoginRequestModel LoginRequestModel { get; set; } = new();
        private LoginResponseModel LoginResponseModel { get; set; } = new();
        private string ErrorMessage = string.Empty;
        private MudTextField<string> pwField1;

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
            if (DateTime.Now.Year == 2026)
            {
                ErrorMessage = "Unable to login. Please contact your systems administrator.";

                return;
            }

            User = await GetUserPrincipal();

            if (User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateToCustom("/");
            }

            if (LoginRequestModel.Email != null && LoginRequestModel.Password != null)
                LoginResponseModel = await _accountService.Login(LoginRequestModel);

            if (!string.IsNullOrEmpty(LoginResponseModel.ErrorMessage))
                ErrorMessage = LoginResponseModel.ErrorMessage;

            StateHasChanged();
            await base.OnAfterRenderAsync(firstRender);
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

        private async Task LoginUser()
        {
            // Do nothing as login process happen in OnAfterRenderAsync() method.
        }

        async Task<ClaimsPrincipal> GetUserPrincipal()
        {
            return (await AuthState).User;
        }
    }
}
