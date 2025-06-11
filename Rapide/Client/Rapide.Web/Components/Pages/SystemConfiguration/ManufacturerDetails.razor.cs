using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class ManufacturerDetails
    {
        #region Parameters
        [Parameter]
        public string? ManufacturerId { get; set; }
        #endregion

        #region Dependency Injection
        #endregion

        #region Private Properties
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task OnSaveClick()
        {
            await form.Validate();
            if (!form.IsValid)
                return;
        }
    }
}