using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rapide.Entities;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class ServicesDetails
    {
        #region Parameters
        [Parameter]
        public string? ServicesId { get; set; }
        #endregion

        #region Dependency Injection
        #endregion

        #region Private Properties
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