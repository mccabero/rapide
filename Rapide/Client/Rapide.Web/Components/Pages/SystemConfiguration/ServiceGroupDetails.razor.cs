using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class ServiceGroupDetails
    {
        #region Parameters
        [Parameter]
        public string? ServiceGroupId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IServiceGroupService ServiceGroupService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private ServiceGroupDTO ServiceGroupRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(ServiceGroupId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            if (isEditMode)
            {
                // Get data by id
                ServiceGroupRequestModel = await ServiceGroupService.GetAsync(x => x.Id == int.Parse(ServiceGroupId));
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
                    bool isEditMode = !string.IsNullOrEmpty(ServiceGroupId);

                    if (!isEditMode) // create mode
                    {
                        ServiceGroupRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ServiceGroupRequestModel.CreatedDateTime = DateTime.Now;
                        ServiceGroupRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ServiceGroupRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await ServiceGroupService.CreateAsync(ServiceGroupRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Service Group Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/service-groups");

                    }
                    else // update mode
                    {
                        ServiceGroupRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ServiceGroupRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await ServiceGroupService.UpdateAsync(ServiceGroupRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Service Group Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/service-groups");
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
            NavigationManager.NavigateToCustom("/configurations/service-groups");
        }
    }
}