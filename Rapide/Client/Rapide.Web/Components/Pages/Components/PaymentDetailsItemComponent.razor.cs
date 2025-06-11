using Microsoft.AspNetCore.Components;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Repositories.Repos;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class PaymentDetailsItemComponent
    {
        #region Parameters
        [Parameter]
        public List<PaymentDetailsDTO> PaymentDetailsParam { get; set; }
        [Parameter]
        public List<InvoiceDTO> InvoiceListParam { get; set; }

        [Parameter]
        public EventCallback<List<PaymentDetailsDTO>> OnDataChanged { get; set; }
        [Parameter]
        public bool IsPaymentLocked { get; set; }
        #endregion

        #region Dependency Injection
        [Inject]
        private IPaymentDetailsService PaymentDetailsService { get; set; }
        [Inject] 
        private IParameterService ParameterService { get; set; }
        [Inject]
        private IDepositService DepositService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private List<PaymentDetailsDTO> PaymentDetailsList { get; set; } = new();
        private List<ParameterDTO> PaymentTypeList { get; set; } = new();
        
        #endregion

        protected override async Task OnInitializedAsync()
        {
            var allParams = await ParameterService.GetAllParameterAsync();
            PaymentTypeList = allParams.Where(x => x.ParameterGroup.Name.Equals(Constants.ParameterType.PaymentTypeParam)).ToList();

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task RemoveValue(PaymentDetailsDTO i)
        {
            PaymentDetailsParam.Remove(i);
            await OnDataChanged.InvokeAsync(PaymentDetailsParam);
        } 
        
        private void OnAddServiceClick()
        {
            PaymentDetailsParam.Add(new PaymentDetailsDTO());
            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            var paymentId = PaymentDetailsParam.FirstOrDefault().PaymentId;
            bool isEditMode = paymentId > 0;
            var returnUrl = isEditMode
                ? $"/operations/payments/{paymentId}"
                : "/operations/payments/add";

            NavigationManager.NavigateToCustom($"/configurations/parameters/add?returnUrl={returnUrl}");
        }

        private async Task OnAmountPaidChanged(PaymentDetailsDTO element, decimal i)
        {
            element.AmountPaid = i;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(PaymentDetailsParam);

            StateHasChanged();
        }

        private async Task OnInvoiceChanged(PaymentDetailsDTO element, InvoiceDTO i)
        {
            var depositList = await DepositService.GetAllDepositAsync();
            if (depositList != null)
            {
                var depositInfo = depositList.Where(x => x.JobStatusId == 1 && x.JobOrderId == i.JobOrder.Id).ToList();

                element.DepositAmount = depositInfo.Any() ? depositInfo.FirstOrDefault().DepositAmount : 0;
            }
           

            element.Invoice = i;

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(PaymentDetailsParam);

            StateHasChanged();
        }

        private async Task<IEnumerable<ParameterDTO>> SearchParam(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return PaymentTypeList;

            return PaymentTypeList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<InvoiceDTO>> SearchInvoice(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return InvoiceListParam;

            return InvoiceListParam.Where(i => i.InvoiceNo.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}