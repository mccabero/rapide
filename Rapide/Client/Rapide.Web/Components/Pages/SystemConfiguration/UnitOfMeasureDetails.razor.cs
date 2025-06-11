using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class UnitOfMeasureDetails
    {
        #region Parameters
        [Parameter]
        public string? UnitOfMeasureId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IUnitOfMeasureService UnitOfMeasureService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private UnitOfMeasureDTO UnitOfMeasureRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(UnitOfMeasureId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            if (isEditMode)
            {
                UnitOfMeasureRequestModel = await UnitOfMeasureService.GetAsync(x => x.Id == int.Parse(UnitOfMeasureId));
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
                    bool isEditMode = !string.IsNullOrEmpty(UnitOfMeasureId);

                    if (!isEditMode) // create mode
                    {
                        UnitOfMeasureRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        UnitOfMeasureRequestModel.CreatedDateTime = DateTime.Now;
                        UnitOfMeasureRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        UnitOfMeasureRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await UnitOfMeasureService.CreateAsync(UnitOfMeasureRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Unit of Measure Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/unit-of-measures");

                    }
                    else // update mode
                    {
                        UnitOfMeasureRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        UnitOfMeasureRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await UnitOfMeasureService.UpdateAsync(UnitOfMeasureRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Unit of Measure Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/unit-of-measures");
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
            NavigationManager.NavigateToCustom("/configurations/unit-of-measures");
        }
    }
}