using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class SupplierDetails
    {
        #region Parameters
        [Parameter]
        public string? SupplierId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private ISupplierService SupplierService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private SupplierDTO SupplierRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(SupplierId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            if (isEditMode)
            {
                SupplierRequestModel = await SupplierService.GetAsync(x => x.Id == int.Parse(SupplierId));
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
                    bool isEditMode = !string.IsNullOrEmpty(SupplierId);

                    if (!isEditMode) // create mode
                    {
                        SupplierRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        SupplierRequestModel.CreatedDateTime = DateTime.Now;
                        SupplierRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        SupplierRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await SupplierService.CreateAsync(SupplierRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Supplier Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/suppliers");

                    }
                    else // update mode
                    {
                        SupplierRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        SupplierRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await SupplierService.UpdateAsync(SupplierRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Supplier Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/suppliers");
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
            NavigationManager.NavigateToCustom("/configurations/suppliers");
        }
    }
}