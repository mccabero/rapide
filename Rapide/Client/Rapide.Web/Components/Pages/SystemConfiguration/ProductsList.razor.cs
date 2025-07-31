using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class ProductsList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IProductService? ProductService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private MudDataGrid<ProductModel> dataGrid;
        private string searchString;
        private List<ProductModel> ProductRequestModel = new List<ProductModel>();

        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }

        private bool isBigThreeRoles = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isBigThreeRoles = TokenHelper.IsBigThreeRolesWithoutSupervisor(await AuthState);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var dataList = await ProductService.GetAllProductAsync();

            if (dataList == null)
            {
                IsLoading = false;
                return;
            }

            foreach (var ul in dataList)
            {
                ProductRequestModel.Add(new ProductModel()
                {
                    Id = ul.Id,
                    Name = ul.Name,
                    DisplayName = ul.DisplayName,
                    Description = ul.Description,
                    PartNo = ul.PartNo,
                    ProductGroup = ul.ProductGroup.Map<ProductGroupModel>(),
                    ProductCategory = ul.ProductCategory.Map<ProductCategoryModel>(),
                    UnitOfMeasure = ul.UnitOfMeasure.Map<UnitOfMeasureModel>(),
                    Manufacturer = ul.Manufacturer.Map<ManufacturerModel>(),
                    ExpirationDateTime = ul.ExpirationDateTime,
                    PurchaseCost = ul.PurchaseCost,
                    MarkupRate = ul.MarkupRate,
                    SellingPrice = ul.SellingPrice,
                    StorageLocation = ul.StorageLocation
                });
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task<GridData<ProductModel>> ServerReload(GridState<ProductModel> state)
        {
            IEnumerable<ProductModel> data = new List<ProductModel>();
            data = ProductRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (!string.IsNullOrEmpty(element.Description))
                {
                    if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                if (element.ProductGroup.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.ProductCategory.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Manufacturer.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.PurchaseCost.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.SellingPrice.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (!string.IsNullOrEmpty(element.PartNo))
                {
                    if (element.PartNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(ProductModel.Name):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Name
                        );
                        break;
                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<ProductModel>
            {
                TotalItems = totalItems,
                Items = pagedData
            };
        }

        private Task OnSearch(string text)
        {
            searchString = text;
            return dataGrid.ReloadServerData();
        }

        private void OnAddClick()
        {
            NavigationManager.NavigateToCustom("/configurations/products/add");
        }

        private async Task OnDeleteClick(ProductModel productModel)
        {
            try
            {
                if (productModel != null)
                {
                    bool? result = await mbox.ShowAsync();
                    var proceed = result == null ? false : true;

                    if (proceed)
                    {
                        IsLoading = true;

                        await ProductService.DeleteAsync(productModel.Id);
                        SnackbarService.Add("Product Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/configurations/products", true);
                    }
                }
            }
            catch (Exception)
            {
                mBoxCustomMessage = "Unable to delete the this record. This might be used in another transaction.";
                await mboxError.ShowAsync();

                IsLoading = false;
                StateHasChanged();
                return;
            }
        }
    }
}