using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using System.Data;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class ParameterDetails
    {
        #region Parameters
        [Parameter]
        public string? ParameterId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IParameterService ParameterService { get; set; }
        [Inject]
        private IParameterGroupService ParameterGroupService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private List<ParameterGroupDTO> ParameterGroups { get; set; } = new();
        private MudMessageBox mbox { get; set; }
        private ParameterDTO ParameterRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            ParameterGroups = await ParameterGroupService.GetAllAsync();
            bool isEditMode = !string.IsNullOrEmpty(ParameterId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            if (isEditMode)
            {
                // Get user data by id
                ParameterRequestModel = await ParameterService.GetParameterByIdAsync(int.Parse(ParameterId));
                form.Disabled = isViewOnly;
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private void OnAddNewItemClick()
        {
            bool isEditMode = !string.IsNullOrEmpty(ParameterId);
            var returnUrl = isEditMode
                ? $"/configurations/parameters/{ParameterId}"
                : "/configurations/parameters/add";

            NavigationManager.NavigateToCustom($"/configurations/parameter-groups/add?returnUrl={returnUrl}");
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
                    bool isEditMode = !string.IsNullOrEmpty(ParameterId);

                    ParameterRequestModel.ParameterGroupId = ParameterRequestModel.ParameterGroup.Id;

                    if (!isEditMode) // create mode
                    {
                        ParameterRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ParameterRequestModel.CreatedDateTime = DateTime.Now;
                        ParameterRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ParameterRequestModel.UpdatedDateTime = DateTime.Now;

                        ParameterRequestModel.OtherData = ParameterRequestModel.OtherData == null
                            ? string.Empty
                            : ParameterRequestModel.OtherData;

                        await ParameterService.CreateAsync(ParameterRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Parameter Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/parameters");

                    }
                    else // update mode
                    {
                        ParameterRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ParameterRequestModel.UpdatedDateTime = DateTime.Now;

                        ParameterRequestModel.OtherData = ParameterRequestModel.OtherData == null
                            ? string.Empty
                            : ParameterRequestModel.OtherData;

                        // call update endpoint here...
                        await ParameterService.UpdateAsync(ParameterRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Parameter Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/parameters");
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
            NavigationManager.NavigateToCustom("/configurations/parameters");
        }

        private async Task<IEnumerable<ParameterGroupDTO>> Search(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ParameterGroups;

            return ParameterGroups.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}