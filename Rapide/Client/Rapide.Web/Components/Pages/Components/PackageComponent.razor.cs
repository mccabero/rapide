using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class PackageComponent
    {
        #region Parameters
        [Parameter]
        public string PackageIdParam { get; set; }
        [Parameter]
        public PackageDTO PackageRequestModelParam { get; set; } = new();
        #endregion

        #region Dependency Injection
        [Inject]
        private IPackageProductService PackageProductService { get; set; }
        [Inject]
        private IPackageServiceService PackageServiceService { get; set; }
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        #endregion

        #region Private Properties
        private bool IsEditMode = false;
        private bool isAllowOverride = false;
        // From child components
        public List<PackageServiceDTO> PackageServices { get; set; } = new();
        public List<PackageProductDTO> PackageProducts { get; set; } = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsEditMode = !string.IsNullOrEmpty(PackageIdParam);
            isAllowOverride = TokenHelper.IsBigThreeRoles(await AuthState);

            if (IsEditMode)
            {
                var productList = await PackageProductService.GetAllPackageProductByPackageIdAsync(int.Parse(PackageIdParam));
                var serviceList = await PackageServiceService.GetAllPackageServiceByPackageIdAsync(int.Parse(PackageIdParam));

                PackageServices = serviceList;
                PackageProducts = productList;
            }

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }

        private void ServiceItemHasChanged(List<PackageServiceDTO> e)
        {
            PackageServices = e;
            var productTotal = PackageProducts == null
                ? 0
                : PackageProducts.Sum(x => x.Amount);

            var productAndServiceTotal = e.Sum(x => x.Amount) + productTotal;

            PackageRequestModelParam.SubTotal = (decimal)productAndServiceTotal;
            PackageRequestModelParam.TotalAmount = (decimal)productAndServiceTotal;

            PackageRequestModelParam.ServiceList = e;
            PackageRequestModelParam.ProductList = PackageProducts;

            StateHasChanged();
        }
        
        private void ProductItemHasChanged(List<PackageProductDTO> e)
        {
            PackageProducts = e;
            var serviceTotal = PackageServices == null
                ? 0
                : PackageServices.Sum(x => x.Amount);

            var productAndServiceTotal = e.Sum(x => x.Amount) + serviceTotal;
            PackageRequestModelParam.SubTotal = (decimal)productAndServiceTotal;
            PackageRequestModelParam.TotalAmount = (decimal)productAndServiceTotal;

            PackageRequestModelParam.ServiceList = PackageServices;
            PackageRequestModelParam.ProductList = e;

            StateHasChanged();
        }

        private void OnSubTotalChanged(PackageDTO dto, string i)
        {
            PackageRequestModelParam.VAT12 = decimal.Parse(i) * 12 / 100;

            StateHasChanged();
        }
    }
}