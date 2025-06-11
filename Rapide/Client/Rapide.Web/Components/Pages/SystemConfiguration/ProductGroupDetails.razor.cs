using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class ProductGroupDetails
    {
        #region Parameters
        [Parameter]
        public string? ProductGroupId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IProductGroupService ProductGroupService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private ProductGroupDTO ProductGroupRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(ProductGroupId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            if (isEditMode)
            {
                // Get data by id
                ProductGroupRequestModel = await ProductGroupService.GetAsync(x => x.Id == int.Parse(ProductGroupId));
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
                    bool isEditMode = !string.IsNullOrEmpty(ProductGroupId);

                    if (!isEditMode) // create mode
                    {
                        ProductGroupRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ProductGroupRequestModel.CreatedDateTime = DateTime.Now;
                        ProductGroupRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ProductGroupRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await ProductGroupService.CreateAsync(ProductGroupRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Product Group Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/product-groups");

                    }
                    else // update mode
                    {
                        ProductGroupRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ProductGroupRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await ProductGroupService.UpdateAsync(ProductGroupRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Product Group Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/product-groups");
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
            NavigationManager.NavigateToCustom("/configurations/product-groups");
        }
    }
}