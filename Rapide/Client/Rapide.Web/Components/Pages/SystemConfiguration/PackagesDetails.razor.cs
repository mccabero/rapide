using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class PackagesDetails
    {
        #region Parameters
        [Parameter]
        public string? PackageId { get; set; }
        #endregion

        #region Dependency Injection
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IPackageService PackageService { get; set; }
        [Inject]
        private IPackageProductService PackageProductService { get; set; }
        [Inject]
        private IPackageServiceService PackageServiceService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private MudMessageBox mboxCustom { get; set; }
        private MudMessageBox mboxError { get; set; }

        private string mBoxCustomMessage { get; set; }

        private PackageDTO PackageRequestModel { get; set; } = new();

        private bool IsLoading { get; set; }
        private bool IsEditMode { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            IsEditMode = !string.IsNullOrEmpty(PackageId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            if (IsEditMode)
            {
                await ReloadPackageData();
                form.Disabled = isViewOnly;
            }
            else
            {
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

            var isValidated = await ValidateSubComponents();

            if (!isValidated)
                return;

            bool? result = await mbox.ShowAsync();
            var proceedSaving = result == null ? false : true;

            if (proceedSaving)
            {
                try
                {
                    IsLoading = true;
                    bool isEditMode = !string.IsNullOrEmpty(PackageId);

                    if (!isEditMode) // create mode
                    {
                        PackageRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        PackageRequestModel.CreatedDateTime = DateTime.Now;
                        PackageRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        PackageRequestModel.UpdatedDateTime = DateTime.Now;

                        // call create endpoint here...
                        var created = await PackageService.CreateAsync(PackageRequestModel);

                        // Save services
                        foreach (var s in PackageRequestModel.ServiceList)
                        {
                            s.PackageId = created.Id;
                            s.ServiceId = s.Service.Id;
                            s.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            s.CreatedDateTime = DateTime.Now;
                            s.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            s.UpdatedDateTime = DateTime.Now;

                            await PackageServiceService.CreateAsync(s);
                        }

                        // Save products
                        foreach (var p in PackageRequestModel.ProductList)
                        {
                            p.PackageId = created.Id;
                            p.ProductId = p.Product.Id;
                            p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.CreatedDateTime = DateTime.Now;
                            p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.UpdatedDateTime = DateTime.Now;

                            await PackageProductService.CreateAsync(p);
                        }

                        PackageId = created.Id.ToString();
                        await ReloadPackageData();
                        IsLoading = false;

                        SnackbarService.Add("Job Order Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/packages");
                    }
                    else
                    {
                        int packageId = int.Parse(PackageId);

                        PackageRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        PackageRequestModel.UpdatedDateTime = DateTime.Now;

                        await PackageService.UpdateAsync(PackageRequestModel);

                        var packageServiceList = await PackageServiceService.GetAllPackageServiceByPackageIdAsync(packageId);
                        var packageProductList = await PackageProductService.GetAllPackageProductByPackageIdAsync(packageId);

                        if (packageServiceList != null && packageServiceList.Any())
                        {

                            foreach (var del in packageServiceList)
                            {
                                //if (!estimateSericeListForSkipDelete.Where(x => x.Id == del.Id).Any())
                                await PackageServiceService.DeleteAsync(del.Id);
                            }

                        }

                        if (packageProductList != null && packageProductList.Any())
                        {
                            foreach (var del in packageProductList)
                            {
                                //if (!estiamteProductListForSkipDelete.Where(x => x.Id == del.Id).Any())
                                await PackageProductService.DeleteAsync(del.Id);
                            }

                        }

                        // Save services
                        foreach (var s in PackageRequestModel.ServiceList)
                        {
                            s.Id = 0;
                            s.ServiceId = s.Service.Id;
                            s.PackageId = packageId;
                            s.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            s.CreatedDateTime = DateTime.Now;
                            s.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            s.UpdatedDateTime = DateTime.Now;

                            await PackageServiceService.CreateAsync(s);
                        }

                        // Detele all current products? Update also include insert inside
                        // Save products -> need to check for logic if the product is deleted.
                        foreach (var p in PackageRequestModel.ProductList)
                        {
                            p.Id = 0;
                            p.ProductId = p.Product.Id;
                            p.PackageId = packageId;
                            p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.CreatedDateTime = DateTime.Now;
                            p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.UpdatedDateTime = DateTime.Now;
                            await PackageProductService.CreateAsync(p);
                        }

                        await ReloadPackageData();

                        SnackbarService.Add("Package Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/packages");
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
            NavigationManager.NavigateToCustom("/configurations/packages");
        }

        private async Task<bool> ValidateSubComponents()
        {
            if (PackageRequestModel.ProductList == null || !PackageRequestModel.ProductList.Any())
            {
                // TODO: Ask if product is required in Package
                mBoxCustomMessage = "Products is required!";
                await mboxError.ShowAsync();

                return false;
            }

            if (PackageRequestModel.ProductList.Where(x => x.Product.Id.Equals(0)).Any())
            {
                mBoxCustomMessage = "No selected product or product name is empty!";
                await mboxError.ShowAsync();

                return false;
            }

            if (PackageRequestModel == null || !PackageRequestModel.ServiceList.Any())
            {
                // TODO: Ask if service is required in Package
                mBoxCustomMessage = "Service is required!";
                await mboxError.ShowAsync();

                return false;
            }

            if (PackageRequestModel.ServiceList.Where(x => x.Service.Id.Equals(0)).Any())
            {
                mBoxCustomMessage = "No selected service or service name is empty!";
                await mboxError.ShowAsync();

                return false;
            }

            return true;
        }

        private async Task ReloadPackageData()
        {
            // Get data by id
            PackageRequestModel = await PackageService.GetByIdAsync(int.Parse(PackageId));

            PackageRequestModel.ServiceList = await PackageServiceService.GetAllPackageServiceByPackageIdAsync(int.Parse(PackageId));
            PackageRequestModel.ProductList = await PackageProductService.GetAllPackageProductByPackageIdAsync(int.Parse(PackageId));

        }
    }
}
