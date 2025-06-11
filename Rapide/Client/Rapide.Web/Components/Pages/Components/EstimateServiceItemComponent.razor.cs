using Microsoft.AspNetCore.Components;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class EstimateServiceItemComponent
    {
        #region Parameters
        [Parameter]
        public List<EstimateServiceDTO> EstimateServicesParam { get; set; }

        [Parameter]
        public EventCallback<List<EstimateServiceDTO>> OnDataChanged { get; set; }
        [Parameter]
        public bool IsEstimateLocked { get; set; }
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

        private async Task RemoveValue(EstimateServiceDTO i)
        {
            EstimateServicesParam.Remove(i);
            await OnDataChanged.InvokeAsync(EstimateServicesParam);
        }
        
        private void OnAddServiceClick()
        {
            EstimateServicesParam.Add(new EstimateServiceDTO()
            {
                Hours = 1,
                IsRequired = true,
                Estimate = new EstimateDTO(),
                Service = new ServiceDTO()
            });
            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            var estimateId = EstimateServicesParam.FirstOrDefault().EstimateId;
            bool isEditMode = estimateId > 0;
            var returnUrl = isEditMode
                ? $"/operations/estimates/{estimateId}"
                : "/operations/estimates/add";

            NavigationManager.NavigateToCustom($"/configurations/services/add?returnUrl={returnUrl}");
        }

        private async Task OnRateChanged(EstimateServiceDTO element, decimal i)
        {
            element.Rate = i;
            element.Amount = element.Hours * i;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(EstimateServicesParam);

            StateHasChanged();
        }

        private async Task OnHoursChanged(EstimateServiceDTO element, decimal i)
        {
            element.Hours = i;
            element.Amount = element.Rate * i;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(EstimateServicesParam);

            StateHasChanged();
        }

        private async Task OnServiceChanged(EstimateServiceDTO element, ServiceDTO dto)
        {
            element.Service = dto;
            element.Rate = dto.StandardRate;
            element.Amount = element.Hours * dto.StandardRate;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(EstimateServicesParam);

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