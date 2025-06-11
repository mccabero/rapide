using Microsoft.AspNetCore.Components;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class JobOrderServiceItemComponent
    {
        #region Parameters
        [Parameter]
        public List<JobOrderServiceDTO> JobOrderServicesParam { get; set; }

        [Parameter]
        public EventCallback<List<JobOrderServiceDTO>> OnDataChanged { get; set; }
        [Parameter]
        public bool IsJobOrderLocked { get; set; }
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

        private async Task RemoveValue(JobOrderServiceDTO i)
        {
            JobOrderServicesParam.Remove(i);
            await OnDataChanged.InvokeAsync(JobOrderServicesParam);
        }
        
        private void OnAddServiceClick()
        {
            JobOrderServicesParam.Add(new JobOrderServiceDTO()
            {
                Hours = 1,
                JobOrder = new JobOrderDTO(),
                Service = new ServiceDTO()
            });
            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            var jobOrderId = JobOrderServicesParam.FirstOrDefault().JobOrderId;
            bool isEditMode = jobOrderId > 0;
            var returnUrl = isEditMode
                ? $"/operations/job-orders/{jobOrderId}"
                : "/operations/job-orders/add";

            NavigationManager.NavigateToCustom($"/configurations/services/add?returnUrl={returnUrl}");
        }

        private async Task OnRateChanged(JobOrderServiceDTO element, decimal i)
        {
            element.Rate = i;
            element.Amount = element.Hours * i;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(JobOrderServicesParam);

            StateHasChanged();
        }

        private async Task OnHoursChanged(JobOrderServiceDTO element, decimal i)
        {
            element.Hours = i;
            element.Amount = element.Rate * i;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(JobOrderServicesParam);

            StateHasChanged();
        }

        private async Task OnServiceChanged(JobOrderServiceDTO element, ServiceDTO dto)
        {
            element.Service = dto;
            element.Rate = dto.StandardRate;
            element.Amount = element.Hours * dto.StandardRate;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(JobOrderServicesParam);

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