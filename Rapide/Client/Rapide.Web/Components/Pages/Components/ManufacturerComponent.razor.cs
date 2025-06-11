using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class ManufacturerComponent
    {
        #region Parameters
        [Parameter]
        public string? ManufacturerId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IManufacturerService ManufacturerService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private ManufacturerDTO ManufacturerRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(ManufacturerId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            if (isEditMode)
            {
                // Get data by id
                ManufacturerRequestModel = await ManufacturerService.GetAsync(x => x.Id == int.Parse(ManufacturerId));
                form.Disabled = isViewOnly;
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task OnSaveClick()
        {
            await form.Validate();
            if (!form.IsValid)
                return;

            bool? result = await mbox.ShowAsync();
            var proceedSaving = result == null ? false : true;

            if (proceedSaving)
            {
                try
                {
                    IsLoading = true;
                    bool isEditMode = !string.IsNullOrEmpty(ManufacturerId);

                    if (!isEditMode) // create mode
                    {
                        ManufacturerRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ManufacturerRequestModel.CreatedDateTime = DateTime.Now;
                        ManufacturerRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ManufacturerRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await ManufacturerService.CreateAsync(ManufacturerRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Manufacturer Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/manufacturers");

                    }
                    else // update mode
                    {
                        ManufacturerRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ManufacturerRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await ManufacturerService.UpdateAsync(ManufacturerRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Manufacturer Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/manufacturers");
                    }
                }
                catch (Exception ex)
                {
                    SnackbarService.Add(
                        $"Error occurred while processing the transaction. Please contact your systems administrator.{Environment.NewLine}" +
                        $"Error Message: {ex.Message} ",
                        Severity.Error,
                        config => { config.ShowCloseIcon = true; });

                    IsLoading = false;
                }
            }
        }

        private async Task OnCancelClick()
        {
            NavigationManager.NavigateToCustom("/configurations/manufacturers");
        }
    }
}