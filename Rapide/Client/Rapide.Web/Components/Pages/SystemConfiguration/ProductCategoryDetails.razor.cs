using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class ProductCategoryDetails
    {
        #region Parameters
        [Parameter]
        public string? ProductCategoryId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IProductCategoryService ProductCategoryService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private ProductCategoryDTO ProductCategoryRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(ProductCategoryId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            if (isEditMode)
            {
                // Get data by id
                ProductCategoryRequestModel = await ProductCategoryService.GetAsync(x => x.Id == int.Parse(ProductCategoryId));
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
                    bool isEditMode = !string.IsNullOrEmpty(ProductCategoryId);

                    if (!isEditMode) // create mode
                    {
                        ProductCategoryRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ProductCategoryRequestModel.CreatedDateTime = DateTime.Now;
                        ProductCategoryRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ProductCategoryRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await ProductCategoryService.CreateAsync(ProductCategoryRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Product Category Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/product-categories");

                    }
                    else // update mode
                    {
                        ProductCategoryRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ProductCategoryRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await ProductCategoryService.UpdateAsync(ProductCategoryRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Product Category Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/product-categories");
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
            NavigationManager.NavigateToCustom("/configurations/product-categories");
        }
    }
}