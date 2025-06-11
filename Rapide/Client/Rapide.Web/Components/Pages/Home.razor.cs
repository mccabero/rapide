using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rapide.Contracts.Services;
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

        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        
    }
}
