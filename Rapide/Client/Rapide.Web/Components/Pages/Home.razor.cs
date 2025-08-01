using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rapide.Contracts.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using System.Threading.Tasks;

namespace Rapide.Web.Components.Pages
{
    public partial class Home
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        #endregion

        #region Private Properties
        private bool isTechnician = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            isTechnician = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.SeniorTechnician)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.JuniorTechnician);

            await base.OnInitializedAsync();
        }

        
    }
}
