using Microsoft.AspNetCore.Components;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class QuickSalesProductItemComponent
    {
        #region Parameters
        [Parameter]
        public List<QuickSalesProductDTO> QuickSalesProductParam { get; set; }

        [Parameter]
        public EventCallback<List<QuickSalesProductDTO>> OnDataChanged { get; set; }
        [Parameter]
        public bool IsQuickSalesLocked { get; set; }
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

        private async Task RemoveValue(QuickSalesProductDTO i)
        {
            QuickSalesProductParam.Remove(i);
            await OnDataChanged.InvokeAsync(QuickSalesProductParam);
        }
        
        private void OnAddProductClick()
        {
            QuickSalesProductParam.Add(new QuickSalesProductDTO()
            {
                Qty = 1,
                QuickSales = new QuickSalesDTO(),
                Product = new ProductDTO()
            });

            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            var QuickSalesId = QuickSalesProductParam.FirstOrDefault().QuickSalesId;
            bool isEditMode = QuickSalesId > 0;
            var returnUrl = isEditMode
                ? $"/operations/quick-sales/{QuickSalesId}"
                : "/operations/quick-sales/add";

            NavigationManager.NavigateToCustom($"/configurations/products/add?returnUrl={returnUrl}");
        }

        private async Task OnPriceChanged(QuickSalesProductDTO element, decimal i)
        {
            element.Price = i;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(QuickSalesProductParam);

            StateHasChanged();
        }

        private async Task OnQtyChanged(QuickSalesProductDTO element, int i)
        {
            element.Qty = i;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(QuickSalesProductParam);

            StateHasChanged();
        }

        private async Task OnProductChanged(QuickSalesProductDTO element, ProductDTO dto)
        {
            element.Product = dto;
            element.Price = dto.SellingPrice;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(QuickSalesProductParam);

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