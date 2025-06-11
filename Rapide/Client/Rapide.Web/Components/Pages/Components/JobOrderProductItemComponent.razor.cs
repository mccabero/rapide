using Microsoft.AspNetCore.Components;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class JobOrderProductItemComponent
    {
        #region Parameters
        [Parameter]
        public List<JobOrderProductDTO> JobOrderProductParam { get; set; }

        [Parameter]
        public EventCallback<List<JobOrderProductDTO>> OnDataChanged { get; set; }
        [Parameter]
        public bool IsJobOrderLocked { get; set; }
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

        private async Task RemoveValue(JobOrderProductDTO i)
        {
            JobOrderProductParam.Remove(i);
            await OnDataChanged.InvokeAsync(JobOrderProductParam);
        }
        
        private void OnAddProductClick()
        {
            JobOrderProductParam.Add(new JobOrderProductDTO()
            {
                Qty = 1,
                JobOrder = new JobOrderDTO(),
                Product = new ProductDTO()
            });

            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            var jobOrderId = JobOrderProductParam.FirstOrDefault().JobOrderId;
            bool isEditMode = jobOrderId > 0;
            var returnUrl = isEditMode
                ? $"/operations/job-orders/{jobOrderId}"
                : "/operations/job-orders/add";

            NavigationManager.NavigateToCustom($"/configurations/products/add?returnUrl={returnUrl}");
        }

        private async Task OnPriceChanged(JobOrderProductDTO element, decimal i)
        {
            element.Price = i;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(JobOrderProductParam);

            StateHasChanged();
        }

        private async Task OnQtyChanged(JobOrderProductDTO element, int i)
        {
            element.Qty = i;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(JobOrderProductParam);

            StateHasChanged();
        }

        private async Task OnProductChanged(JobOrderProductDTO element, ProductDTO dto)
        {
            element.Product = dto;
            element.Price = dto.SellingPrice;
            element.Amount = element.Price * element.Qty;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(JobOrderProductParam);

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