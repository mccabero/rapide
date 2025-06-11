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
    public partial class ProductDetails
    {
        #region Parameters
        [Parameter]
        public string? ProductId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IProductService ProductService { get; set; }
        [Inject]
        private IProductGroupService ProductGroupService { get; set; }
        [Inject]
        private IProductCategoryService ProductCategoryService { get; set; }
        [Inject]
        private IUnitOfMeasureService UnitOfMeasureService { get; set; }
        [Inject]
        private IManufacturerService ManufacturerService { get; set; }
        [Inject]
        private ISupplierService SupplierService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private ProductDTO ProductRequestModel { get; set; } = new();
        private List<ProductCategoryDTO> ProductCategoryList { get; set; } = new();
        private List<ProductGroupDTO> ProductGroupList { get; set; } = new();
        private List<UnitOfMeasureDTO> UnitOfMeasureList { get; set; } = new();
        private List<ManufacturerDTO> ManufacturerList { get; set; } = new();
        private List<SupplierDTO> SupplierList { get; set; } = new();

        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;
        private bool isAllowOverride = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(ProductId);
            isAllowOverride = TokenHelper.IsBigThreeRoles(await AuthState);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            ProductGroupList = await ProductGroupService.GetAllAsync();
            ProductCategoryList = await ProductCategoryService.GetAllAsync();
            UnitOfMeasureList = await UnitOfMeasureService.GetAllAsync();
            ManufacturerList = await ManufacturerService.GetAllAsync();
            SupplierList = await SupplierService.GetAllAsync();

            if (isEditMode)
            {
                // Get data by id
                ProductRequestModel = await ProductService.GetProductByIdAsync(int.Parse(ProductId));
                form.Disabled = isViewOnly;
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private void OnPurchaseCostChanged(ProductDTO dto, decimal i)
        {
            ProductRequestModel.PurchaseCost = i;
            ProductRequestModel.MarkupRate = i / ProductRequestModel.PurchaseCost;

            StateHasChanged();
        }

        private void OnSellingPriceChanged(ProductDTO dto, decimal i)
        {
            ProductRequestModel.MarkupRate = i / ProductRequestModel.PurchaseCost;
            ProductRequestModel.SellingPrice = i;
            
            StateHasChanged();
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
                    bool isEditMode = !string.IsNullOrEmpty(ProductId);

                    ProductRequestModel.ProductCategoryId = ProductRequestModel.ProductCategory.Id;
                    ProductRequestModel.ProductGroupId = ProductRequestModel.ProductGroup.Id;
                    ProductRequestModel.UnitOfMeasureId = ProductRequestModel.UnitOfMeasure.Id;
                    ProductRequestModel.ManufacturerId = ProductRequestModel.Manufacturer.Id;
                    ProductRequestModel.SupplierId = ProductRequestModel.Supplier.Id;

                    if (!isEditMode) // create mode
                    {
                        ProductRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ProductRequestModel.CreatedDateTime = DateTime.Now;
                        ProductRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ProductRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await ProductService.CreateAsync(ProductRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Product Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/products");

                    }
                    else // update mode
                    {
                        ProductRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ProductRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await ProductService.UpdateAsync(ProductRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Product Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/products");
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
            NavigationManager.NavigateToCustom("/configurations/products");
        }

        private void OnAddNewItemClick(ProductDialogType dialogType)
        {
            bool isEditMode = !string.IsNullOrEmpty(ProductId);
            var returnUrl = isEditMode
                ? $"/configurations/products/{ProductId}"
                : "/configurations/products/add";

            if (dialogType == ProductDialogType.ProductGroup)
                NavigationManager.NavigateToCustom($"/configurations/product-groups/add?returnUrl={returnUrl}");
            else if (dialogType == ProductDialogType.ProductCategory)
                NavigationManager.NavigateToCustom($"/configurations/product-categories/add?returnUrl={returnUrl}");
            else if (dialogType == ProductDialogType.UnitOfMeasure)
                NavigationManager.NavigateToCustom($"/configurations/unit-of-measures/add?returnUrl={returnUrl}");
            else if (dialogType == ProductDialogType.Manufacturer)
                NavigationManager.NavigateToCustom($"/configurations/manufacturers/add?returnUrl={returnUrl}");
            else if (dialogType == ProductDialogType.Supplier)
                NavigationManager.NavigateToCustom($"/configurations/suppliers/add?returnUrl={returnUrl}");
        }

        #region Search MudAutoComplete
        private async Task<IEnumerable<ProductGroupDTO>> SearchProductGroup(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ProductGroupList;

            return ProductGroupList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ProductCategoryDTO>> SearchProductCategory(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ProductCategoryList;

            return ProductCategoryList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<UnitOfMeasureDTO>> SearchUnitOfMeasure(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return UnitOfMeasureList;

            return UnitOfMeasureList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ManufacturerDTO>> SearchManufacturer(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ManufacturerList;

            return ManufacturerList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<SupplierDTO>> SearchSupplier(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return SupplierList;

            return SupplierList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
        #endregion
    }
}
