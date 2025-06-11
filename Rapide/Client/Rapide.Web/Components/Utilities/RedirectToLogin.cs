using Microsoft.AspNetCore.Components;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Utilities
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        protected override void OnAfterRender(bool firstRender)
        {
            NavigationManager.NavigateToCustom("/login", true);
        }
    }
}
