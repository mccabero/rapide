using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Customers
{
    public partial class CustomerDetails
    {
        #region Parameters
        [Parameter]
        public string? CustomerId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        #endregion

        #region Private Properties
        bool isReadOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            // Add allowed role here as needed
            //var readOnlyRoles = new List<string>();
            //readOnlyRoles.Add(Constants.UserRoles.Cashier);

            //var roleExistInList = TokenHelper.RoleIsInList(await AuthState, readOnlyRoles);
            //isReadOnly = roleExistInList;

            await base.OnInitializedAsync();
        }
    }
}