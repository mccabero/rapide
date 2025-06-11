using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class MudDialogComponent
    {
        #region Parameters
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        #endregion

        #region Dependency Injection
        #endregion

        #region Private Properties
        #endregion

        private void Submit() 
            => MudDialog.Close(DialogResult.Ok(true));

        private void Cancel() 
            => MudDialog.Cancel();
    }
}
