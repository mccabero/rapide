using Microsoft.AspNetCore.Components;
using Rapide.DTO;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class PaymentInvoiceItemComponent
    {
        [Parameter]
        public List<InvoiceDTO> InvoiceListParam { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
