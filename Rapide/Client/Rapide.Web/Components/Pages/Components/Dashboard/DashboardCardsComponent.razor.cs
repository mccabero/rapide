using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.Services;
using Rapide.Web.Helpers;
using System.Threading.Tasks;

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
        #endregion

        protected override async Task OnInitializedAsync()
        {
            // As per owner, cards should display only the same day.
            //var firstDayOfTheMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            _dateRange.Start = DateTime.Now.Date;
            _dateRange.End = DateTime.Now.Date;

            await ReloadDashboardData();

            await base.OnInitializedAsync();
        }

        private async Task ReloadDashboardData()
        {
            IsLoading = true;

            var customers = await CustomerService.GetAllAsync();
            var vehicles = await VehicleService.GetAllVehicleAsync();
            var estimates = await EstimateService.GetAllEstimateAsync();
            var jobOrders = await JobOrderService.GetAllJobOrderAsync();

            customers = customers.Where(x => x.CreatedDateTime >= _dateRange.Start &&  x.CreatedDateTime <= _dateRange.End).ToList();
            vehicles = vehicles.Where(x => x.CreatedDateTime >= _dateRange.Start && x.CreatedDateTime <= _dateRange.End).ToList();
            estimates = estimates.Where(x => x.CreatedDateTime >= _dateRange.Start && x.CreatedDateTime <= _dateRange.End).ToList();
            jobOrders = jobOrders.Where(x => x.CreatedDateTime >= _dateRange.Start && x.CreatedDateTime <= _dateRange.End).ToList();

            customerCount = customers == null ? 0 : customers.Count;
            vehiclesCount = vehicles == null ? 0 : vehicles.Count;
            estimateCount = estimates == null ? 0 : estimates.Count;
            jobOrdersCount = jobOrders == null ? 0 : jobOrders.Count;

            isAllowOverride = TokenHelper.IsBigThreeRolesWithoutSupervisor(await AuthState);

            await GetDiscount();
            await GetExpenses();
            await GetNetSales();
            await GetProfit();

            IsLoading = false;
        }

        private async Task OnCardFilterApply()
        {
            await ReloadDashboardData();
        }

        private async Task GetNetSales()
        {
            var paymentsData = await PaymentService.GetAllPaymentAsync();
            var paymentDetailsData = await PaymentDetailsService.GetAllPaymentDetailsAsync();
            var quickSalesData = await QuickSalesService.GetAllQuickSalesAsync();

            var filteredQuickSales = quickSalesData.Where(x => x.TransactionDate.Value.Date >= _dateRange.Start && x.TransactionDate.Value.Date <= _dateRange.End).ToList();
            var filteredPayment = paymentsData.Where(x => x.PaymentDate.Value.Date >= _dateRange.Start && x.PaymentDate.Value.Date <= _dateRange.End).ToList();
            var paymentIds = filteredPayment.Select(x => x.Id).ToList();

            var filteredPaymentDetails = paymentDetailsData.Where(x => paymentIds.Contains(x.PaymentId)).ToList();

            // total sales
            // less credit card payment total deduction
            // less purchase cost

            var payments = filteredPaymentDetails.Sum(x => x.AmountPaid);
            var quickSales = filteredQuickSales.Sum(x => x.TotalAmount);

            netSalesAmount = payments + quickSales;
        }

        private async Task GetDiscount()
        {
            var invoiceData = await InvoiceService.GetAllInvoiceAsync();

            var filteredInvoice = invoiceData.Where(x => x.InvoiceDate.Value.Date >= _dateRange.Start && x.InvoiceDate.Value.Date <= _dateRange.End).ToList(); 

            var additionalDiscount = filteredInvoice.Sum(x => x.AdditionalDiscount);
            var laborDiscount = filteredInvoice.Sum(x => x.LaborDiscount);
            var productDiscount = filteredInvoice.Sum(x => x.ProductDiscount);

            discountAmount = additionalDiscount + laborDiscount + productDiscount;

        }

        private async Task GetExpenses()
        {
            var expensesData = await ExpensesService.GetAllExpensesAsync();
            var filteredExpenses = expensesData.Where(x => x.ExpenseDateTime.Value.Date >= _dateRange.Start && x.ExpenseDateTime.Value.Date <= _dateRange.End).ToList();

            expenseAmount = filteredExpenses.Sum(x => x.Amount);
        }

        private async Task GetProfit()
        {
            // payroll is not available
            // expenses is available
            profitAmount = decimal.Parse("123.45");
        }
    }
}
