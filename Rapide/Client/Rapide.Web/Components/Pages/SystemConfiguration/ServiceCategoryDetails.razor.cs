using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class ServiceCategoryDetails
    {
        #region Parameters
        [Parameter]
        public string? ServiceCategoryId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IServiceCategoryService ServiceCategoryService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private ServiceCategoryDTO ServiceCategoryRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(ServiceCategoryId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            if (isEditMode)
            {
                // Get data by id
                ServiceCategoryRequestModel = await ServiceCategoryService.GetAsync(x => x.Id == int.Parse(ServiceCategoryId));
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
                    bool isEditMode = !string.IsNullOrEmpty(ServiceCategoryId);

                    if (!isEditMode) // create mode
                    {
                        ServiceCategoryRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ServiceCategoryRequestModel.CreatedDateTime = DateTime.Now;
                        ServiceCategoryRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ServiceCategoryRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await ServiceCategoryService.CreateAsync(ServiceCategoryRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Service Category Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/service-categories");

                    }
                    else // update mode
                    {
                        ServiceCategoryRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ServiceCategoryRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await ServiceCategoryService.UpdateAsync(ServiceCategoryRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Service Category Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/service-categories");
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
            NavigationManager.NavigateToCustom("/configurations/service-categories");
        }
    }
}