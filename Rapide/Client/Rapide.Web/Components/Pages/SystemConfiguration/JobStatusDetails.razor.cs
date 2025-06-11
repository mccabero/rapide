using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class JobStatusDetails
    {
        #region Parameters
        [Parameter]
        public string? JobStatusId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IJobStatusService JobStatusService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private JobStatusDTO JobStatusRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(JobStatusId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            if (isEditMode)
            {
                // Get data by id
                JobStatusRequestModel = await JobStatusService.GetAsync(x => x.Id == int.Parse(JobStatusId));
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
                    bool isEditMode = !string.IsNullOrEmpty(JobStatusId);

                    if (!isEditMode) // create mode
                    {
                        JobStatusRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        JobStatusRequestModel.CreatedDateTime = DateTime.Now;
                        JobStatusRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        JobStatusRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await JobStatusService.CreateAsync(JobStatusRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Job Status Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/job-statuses");

                    }
                    else // update mode
                    {
                        JobStatusRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        JobStatusRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await JobStatusService.UpdateAsync(JobStatusRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Job Status Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/job-statuses");
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
            NavigationManager.NavigateToCustom("/configurations/job-statuses");
        }
    }
}