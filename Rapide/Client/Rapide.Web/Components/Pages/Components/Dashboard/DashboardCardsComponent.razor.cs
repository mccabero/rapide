using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.Services;
using Rapide.Web.Helpers;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Rapide.DTO;
using MudBlazor.Extensions;

namespace Rapide.Web.Components.Pages.Components.Dashboard
{
    public partial class DashboardCardsComponent
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private ICustomerService CustomerService { get; set; }
        [Inject]
        private IVehicleService VehicleService { get; set; }
        [Inject]
        private IEstimateService EstimateService { get; set; }
        [Inject]
        private IJobOrderService JobOrderService { get; set; }
        [Inject]
        private IExpensesService ExpensesService { get; set; }
        [Inject]
        private IInvoiceService InvoiceService { get; set; }
        [Inject]
        private IPaymentService PaymentService { get; set; }
        [Inject]
        private IPaymentDetailsService PaymentDetailsService { get; set; }
        [Inject]
        private IQuickSalesService QuickSalesService { get; set; }
        #endregion

        #region Private Properties
        private bool IsLoading { get; set; }

        private MudDateRangePicker _pickerRange;
        private MudDatePicker _picker;
        private DateRange _dateRange = new DateRange(DateTime.Now.Date, DateTime.Now.AddDays(5).Date);
        private DateTime? _date;

        private int customerCount = 0;
        private int vehiclesCount = 0;
        private int estimateCount = 0;
        private int jobOrdersCount = 0;
        private bool isAllowOverride = false;

        private decimal discountAmount = 0;
        private decimal expenseAmount = 0;
        private decimal netSalesAmount = 0;
        private decimal profitAmount = 0;
        private decimal quickSalesAmount = 0;

        private bool IsBigThreeRoles = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);
            // As per owner, cards should display only the same day.
            //var firstDayOfTheMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            _dateRange.Start = DateTime.Now.Date;
            _dateRange.End = DateTime.Now.Date;

            // TODO: Optimize this...
            await ReloadDashboardData();

            await base.OnInitializedAsync();
        }

        private async Task ReloadDashboardData()
        {
            IsLoading = true;

            // Use inclusive start, exclusive end for correct and efficient comparisons
            var start = _dateRange.Start.Value;
            var endExclusive = _dateRange.End.Value.AddDays(1);

            // Start independent fetches in parallel
            var customersTask = CustomerService.GetAllSummaryAsync();
            var vehiclesTask = VehicleService.GetAllVehicleSummaryAsync();
            var estimatesTask = EstimateService.GetAllEstimateSummaryAsync();
            var jobOrdersTask = JobOrderService.GetAllJobOrderSummaryAsync();

            Task<List<PaymentDTO>> paymentsTask = null;
            Task<List<PaymentDetailsDTO>> paymentDetailsTask = null;
            Task<List<QuickSalesDTO>> quickSalesTask = null;
            Task<List<InvoiceDTO>> invoicesTask = null;
            Task<List<ExpensesDTO>> expensesTask = null;

            if (IsBigThreeRoles)
            {
                paymentsTask = PaymentService.GetAllPaymentAsync();
                paymentDetailsTask = PaymentDetailsService.GetAllPaymentDetailsAsync();
                quickSalesTask = QuickSalesService.GetAllQuickSalesAsync();
                invoicesTask = InvoiceService.GetAllInvoiceAsync();
                expensesTask = ExpensesService.GetAllExpensesAsync();
            }

            // Build task list
            var tasks = new List<Task> { customersTask, vehiclesTask, estimatesTask, jobOrdersTask };
            if (IsBigThreeRoles)
            {
                tasks.Add(paymentsTask);
                tasks.Add(paymentDetailsTask);
                tasks.Add(quickSalesTask);
                tasks.Add(invoicesTask);
                tasks.Add(expensesTask);
            }

            await Task.WhenAll(tasks);

            // Filter and aggregate client-side (prefer server-side endpoints for best performance)
            var customers = (await customersTask).Where(x => x.CreatedDateTime >= start && x.CreatedDateTime < endExclusive).ToList();
            var vehicles = (await vehiclesTask).Where(x => x.CreatedDateTime >= start && x.CreatedDateTime < endExclusive).ToList();
            var estimates = (await estimatesTask).Where(x => x.CreatedDateTime >= start && x.CreatedDateTime < endExclusive).ToList();
            var jobOrders = (await jobOrdersTask).Where(x => x.CreatedDateTime >= start && x.CreatedDateTime < endExclusive).ToList();

            customerCount = customers.Count;
            vehiclesCount = vehicles.Count;
            estimateCount = estimates.Count;
            jobOrdersCount = jobOrders.Count;

            isAllowOverride = TokenHelper.IsBigThreeRoles(await AuthState);

            // Reset aggregates
            discountAmount = 0m;
            expenseAmount = 0m;
            netSalesAmount = 0m;
            profitAmount = 0m;
            quickSalesAmount = 0m;

            if (IsBigThreeRoles)
            {
                var payments = await paymentsTask;
                var paymentDetails = await paymentDetailsTask;
                var quickSales = await quickSalesTask;

                // Filter payments within date range
                var filteredPayments = payments.Where(p => p.PaymentDate.HasValue && p.PaymentDate.Value.Date >= start && p.PaymentDate.Value.Date < endExclusive).ToList();

                // Use HashSet for faster lookup
                var paymentIds = new HashSet<int>(filteredPayments.Select(p => p.Id));

                var filteredPaymentDetails = paymentDetails.Where(pd => paymentIds.Contains(pd.PaymentId)).ToList();
                // Use HashSet for faster lookup
                var invoiceIds = new HashSet<int>(filteredPaymentDetails.Select(p => p.InvoiceId).ToList());

                var filteredQuickSales = quickSales.Where(q => q.TransactionDate.HasValue && q.TransactionDate.Value.Date >= start && q.TransactionDate.Value.Date < endExclusive).ToList();
                
                // Discounts
                var invoices = await invoicesTask;
                var filteredInvoices = invoices.Where(i => invoiceIds.Contains(i.Id)).ToList();

                discountAmount = filteredInvoices.Sum(i => i.AdditionalDiscount + i.LaborDiscount + i.ProductDiscount);

                quickSalesAmount = filteredQuickSales.Sum(x => x.TotalAmount);
                netSalesAmount = filteredPaymentDetails.Sum(x => x.AmountPaid) + quickSalesAmount + discountAmount;

                // Expenses
                var expenses = await expensesTask;
                expenseAmount = expenses.Where(e => e.ExpenseDateTime.HasValue && e.ExpenseDateTime.Value.Date >= start && e.ExpenseDateTime.Value.Date < endExclusive).Sum(e => e.Amount);

                // Profit: simple approximation, refine as needed
                profitAmount = netSalesAmount - expenseAmount - discountAmount;
            }

            IsLoading = false;
            StateHasChanged();
        }

        private async Task OnCardFilterApply()
        {
            await ReloadDashboardData();
        }
    }
}
