using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Services;
using System.Security.Claims;

namespace Rapide.Web.Components.Layout
{
    public partial class MainLayout
    {
        public bool IsLoading { get; set; }

        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        public AccountService _accountService { get; set; }
        [Inject]
        ILocalStorageService LocalStorage { get; set; }
        [Inject]
        IJSRuntime js { get; set; }

        ClaimsPrincipal UserClaims { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            await base.OnInitializedAsync();
            UserClaims = await GetUserPrincipal();

            if (UserClaims.Identity.IsAuthenticated)
            {
                var fullName = UserClaims.Identity.Name;
                // Sample code to get uder details from local storage
                var token = await LocalStorage.GetItemAsync<string>(Constants.LocalToken);

                //var isTokenValid = TokenHelper.IsTokenIsValid(token);

                //if (!isTokenValid)
                //{
                //    await _accountService.Logout();
                //    NavigationManager.NavigateTo($"login");
                //}

                //Start the timer for idle state
                await js.InvokeVoidAsync("initializeInactivityTimer", DotNetObjectReference.Create(this));

            }
            else
            {
                // NavigationManager.NavigateTo($"login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}");
                NavigationManager.NavigateTo($"login");
            }

            IsLoading = false;
        }

        [JSInvokable]
        public async Task Logout()
        {
            var authState = await AuthState;
            if (authState.User.Identity.IsAuthenticated)
            {
                //implement the logging out process here
                //this might be navigation to the Logout page 
                await LogoutUser();
            }
        }

        async Task<ClaimsPrincipal> GetUserPrincipal()
        {
            if (UserClaims == null)
            {
                UserClaims = (await AuthState).User;
            }

            return UserClaims;
        }

        async Task LogoutUser()
        {
            await _accountService.Logout();
            NavigationManager.NavigateToCustom("/login");
        }
    }
}
