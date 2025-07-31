using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Layout
{
    public partial class NavMenu
    {
        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        #endregion

        #region Properties
        private bool isBigThreeRoles = false;
        private bool isCashier = false;
        private bool isOIC = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);
            isCashier = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier);
            isOIC = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.OIC);

            await base.OnInitializedAsync();
        }
    }
}
