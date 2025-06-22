using Microsoft.AspNetCore.Components;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class EstimateProductItemComponent
    {
        #region Parameters
        [Parameter]
        public List<EstimateProductDTO> EstimateProductParam { get; set; }

        [Parameter]
        public EventCallback<List<EstimateProductDTO>> OnDataChanged { get; set; }
        [Parameter]
        public bool IsEstimateLocked { get; set; }
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

        private async Task RemoveValue(EstimateProductDTO i)
        {
            EstimateProductParam.Remove(i);
            await OnDataChanged.InvokeAsync(EstimateProductParam);
        } 
        
        private void OnAddProductClick()
        {
            EstimateProductParam.Add(new EstimateProductDTO()
            {
                Qty = 1,
                IsRequired = true,
                Estimate = new EstimateDTO(),
                Product = new ProductDTO()
            });

            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            var estimateId = EstimateProductParam.FirstOrDefault().EstimateId;
            bool isEditMode = estimateId > 0;
            var returnUrl = isEditMode
                ? $"/operations/estimates/{estimateId}"
                : "/operations/estimates/add";

            NavigationManager.NavigateToCustom($"/configurations/products/add?returnUrl={returnUrl}");
        }

        private async Task OnPriceChanged(EstimateProductDTO element, decimal i)
        {
            element.Price = i;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(EstimateProductParam);

            StateHasChanged();
        }

        private async Task OnQtyChanged(EstimateProductDTO element, int i)
        {
            element.Qty = i;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(EstimateProductParam);

            StateHasChanged();
        }

        private async Task OnProductChanged(EstimateProductDTO element, ProductDTO dto)
        {
            element.Product = dto;
            element.Price = dto.SellingPrice;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(EstimateProductParam);

            StateHasChanged();
        }

        private async Task<IEnumerable<ProductDTO>> SearchProduct(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ProductList;

            return ProductList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)
                || i.PartNo != null && i.PartNo.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}