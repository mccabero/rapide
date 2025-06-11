using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class ParameterGroupDetails
    {
        #region Parameters
        [Parameter]
        public string? ParameterGroupId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IParameterGroupService ParameterGroupService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private ParameterGroupDTO ParameterGroupRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(ParameterGroupId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            if (isEditMode)
            {
                // Get user data by id
                ParameterGroupRequestModel = await ParameterGroupService.GetAsync(x => x.Id == int.Parse(ParameterGroupId));
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
                    bool isEditMode = !string.IsNullOrEmpty(ParameterGroupId);

                    if (!isEditMode) // create mode
                    {
                        ParameterGroupRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ParameterGroupRequestModel.CreatedDateTime = DateTime.Now;
                        ParameterGroupRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ParameterGroupRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await ParameterGroupService.CreateAsync(ParameterGroupRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Parameter Group Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/parameter-groups");

                    }
                    else // update mode
                    {
                        ParameterGroupRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ParameterGroupRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await ParameterGroupService.UpdateAsync(ParameterGroupRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Parameter Group Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/parameter-groups");
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
            NavigationManager.NavigateToCustom("/configurations/parameter-groups");
        }
    }
}