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
        private bool isOwner = false;
        private bool isSupervisor = false;
        private bool isSystemAdministrator = false;
        private bool isBigThreeRoles = false;
        private bool isCashier = false;
        private bool isOIC = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            var roles = TokenHelper.GetUserRoles(await AuthState);

            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    if (!isOwner)
                        isOwner = role.Equals(Constants.UserRoles.Owner, StringComparison.InvariantCultureIgnoreCase);
                    
                    if (!isSupervisor)
                        isSupervisor = role.Equals(Constants.UserRoles.Supervisor, StringComparison.InvariantCultureIgnoreCase);

                    if (!isSystemAdministrator)
                        isSystemAdministrator = role.Equals(Constants.UserRoles.SystemAdministrator, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);
            isCashier = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier);
            isOIC = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.OIC);

            await base.OnInitializedAsync();
        }
    }
}
