using Microsoft.AspNetCore.Components;
using Rapide.DTO;
using MudBlazor;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class PaymentInvoiceItemComponent
    {
        [Parameter]
        public List<InvoiceDTO> InvoiceListParam { get; set; }

        [Parameter]
        public EventCallback<List<InvoiceDTO>> OnDataChanged { get; set; }

        [Inject]
        private ISnackbar SnackbarService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private async Task OnProcessPaymentClick()
        {
            StateHasChanged();

            // Validate that at least one invoice is selected for payment
            if (InvoiceListParam == null || !InvoiceListParam.Any(x => x.PaymentFor))
            {
                SnackbarService.Add("Please select at least one invoice to process payment.", Severity.Warning, config => { config.ShowCloseIcon = true; });
                return;
            }

            if (OnDataChanged.HasDelegate)
                await OnDataChanged.InvokeAsync(InvoiceListParam);
        }
    }
}
