using Microsoft.AspNetCore.Components;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class PackageServiceItemComponent
    {
        #region Parameters
        [Parameter]
        public List<PackageServiceDTO> PackageServicesParam { get; set; }

        [Parameter]
        public EventCallback<List<PackageServiceDTO>> OnDataChanged { get; set; }
        #endregion

        #region Dependency Injection
        [Inject]
        private IServiceService ServiceService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private List<ServiceDTO> ServiceList { get; set; } = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            ServiceList = await ServiceService.GetAllServiceAsync();

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task RemoveValue(PackageServiceDTO i)
        {
            PackageServicesParam.Remove(i);
            await OnDataChanged.InvokeAsync(PackageServicesParam);
        }
        
        private void OnAddServiceClick()
        {
            if (PackageServicesParam == null)
                PackageServicesParam = new List<PackageServiceDTO>();

            PackageServicesParam.Add(new PackageServiceDTO()
            {
                Hours = 1, // default value
                Package = new PackageDTO(),
                Service = new ServiceDTO()
            });
            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            var packageId = PackageServicesParam.FirstOrDefault().PackageId;
            bool isEditMode = packageId > 0;
            var returnUrl = isEditMode
                ? $"/configurations/packages/{packageId}"
                : "/configurations/packages/add";

            NavigationManager.NavigateToCustom($"/configurations/services/add?returnUrl={returnUrl}");
        }

        private async Task OnRateChanged(PackageServiceDTO element, decimal i)
        {
            element.Rate = i;
            element.Amount = element.Hours * i;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(PackageServicesParam);

            StateHasChanged();
        }

        private async Task OnHoursChanged(PackageServiceDTO element, decimal i)
        {
            element.Hours = i;
            element.Amount = element.Rate * i;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(PackageServicesParam);

            StateHasChanged();
        }

        private async Task OnServiceChanged(PackageServiceDTO element, ServiceDTO dto)
        {
            element.Service = dto;
            element.Rate = dto.StandardRate;
            element.Amount = element.Hours * dto.StandardRate;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(PackageServicesParam);

            StateHasChanged();
        }

        private async Task<IEnumerable<ServiceDTO>> SearchService(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ServiceList;

            return ServiceList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}