using Microsoft.AspNetCore.Components;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class PackageProductItemComponent
    {
        #region Parameters
        [Parameter]
        public List<PackageProductDTO> PackageProductParam { get; set; }

        [Parameter]
        public EventCallback<List<PackageProductDTO>> OnDataChanged { get; set; }
        #endregion

        #region Dependency Injection
        [Inject]
        private IProductService ProductService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private List<ProductDTO> ProductList { get; set; } = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            ProductList = await ProductService.GetAllProductAsync();

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task RemoveValue(PackageProductDTO i)
        {
            PackageProductParam.Remove(i);
            await OnDataChanged.InvokeAsync(PackageProductParam);
        }
        
        private void OnAddProductClick()
        {
            PackageProductParam.Add(new PackageProductDTO()
            {
                Qty = 1, // default value
                Package = new PackageDTO(),
                Product = new ProductDTO()
                {
                    Manufacturer = new ManufacturerDTO(),
                    ProductCategory = new ProductCategoryDTO(),
                    ProductGroup = new ProductGroupDTO(),
                    Supplier = new SupplierDTO(),
                    UnitOfMeasure = new UnitOfMeasureDTO(),
                }
            });

            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            var packageId = PackageProductParam.FirstOrDefault().PackageId;
            bool isEditMode = packageId > 0;
            var returnUrl = isEditMode
                ? $"/configurations/packages/{packageId}"
                : "/configurations/packages/add";

            NavigationManager.NavigateToCustom($"/configurations/products/add?returnUrl={returnUrl}");
        }

        private async Task OnPriceChanged(PackageProductDTO element, decimal i)
        {
            element.Price = i;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(PackageProductParam);

            StateHasChanged();
        }

        private async Task OnQtyChanged(PackageProductDTO element, int i)
        {
            element.Qty = i;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(PackageProductParam);

            StateHasChanged();
        }

        private async Task OnProductChanged(PackageProductDTO element, ProductDTO dto)
        {
            element.Product = dto;
            element.Price = dto.SellingPrice;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(PackageProductParam);

            StateHasChanged();
        }

        private async Task<IEnumerable<ProductDTO>> SearchProduct(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ProductList;

            return ProductList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}