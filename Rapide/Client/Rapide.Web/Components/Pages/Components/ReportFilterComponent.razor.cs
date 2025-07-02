using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.PdfReportGenerator;
using Rapide.Web.Services;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class ReportFilterComponent
    {
        #region Parameters
        [Parameter]
        public string ReportType { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        public AccountService _accountService { get; set; }

        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IUserRolesService UserRolesService { get; set; }
        [Inject]
        private IExpensesService ExpensesService { get; set; }
        [Inject]
        private ICustomerService CustomerService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        [Inject]
        private IJobOrderService JobOrderService { get; set; }
        [Inject]
        private IJobOrderPackageService JobOrderPackageService { get; set; }
        [Inject]
        private IJobOrderProductService JobOrderProductService { get; set; }
        [Inject]
        private IJobOrderTechnicianService JobOrderTechnicianService { get; set; }
        [Inject]
        private IJobOrderServiceService JobOrderServiceService { get; set; }
        [Inject]
        private IInvoiceService InvoiceService { get; set; }
        [Inject]
        private IPackageService PackageService { get; set; }
        [Inject]
        private IInvoicePackageService InvoicePackageService { get; set; }
        [Inject]
        private IPaymentService PaymentService { get; set; }
        [Inject]
        private IPaymentDetailsService PaymentDetailsService { get; set; }
        [Inject]
        private IQuickSalesService QuickSalesService { get; set; }
        [Inject]
        private IQuickSalesProductService QuickSalesProductService { get; set; }
        [Inject]
        private IParameterService ParameterService { get; set; }
        [Inject]
        private IDepositService DepositService { get; set; }
        #endregion

        #region Private Properties
        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxCustom { get; set; }
        private MudMessageBox mboxError { get; set; }
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;
        private string reportName = string.Empty;
        private ReportFilterTypes selectedFilter { get; set; }

        private MudDateRangePicker _pickerRange;
        private MudDatePicker _picker;
        private DateRange _dateRange = new DateRange(DateTime.Now.Date, DateTime.Now.AddDays(5).Date);
        private DateTime? _date;
        private bool isBigThreeRoles = false;
        private bool isCashier = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            selectedFilter = ReportFilterTypes.Daily;
            _dateRange = new DateRange(DateTime.Now.Date, DateTime.Now.Date);
            _picker = new MudDatePicker();
            _pickerRange = new MudDateRangePicker();

            if (!isBigThreeRoles)
                _date = DateTime.Now;

            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);
            isCashier = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier);
            await base.OnInitializedAsync();
        }

        private async Task OnPrintClick()
        {
            IsLoading = true;

            if (!isBigThreeRoles)
            {
                if (_date != null)
                {
                    _dateRange.Start = _date;
                    _dateRange.End = _date;
                }
            }

            #region Initial Data Required
            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            // current user
            var id = TokenHelper.GetCurrentUserId(await AuthState);
            UserDTO CurrentUser = await _accountService.GetCurrentLoggedInUser(id);

            var fullName = CurrentUser == null
                ? string.Empty
                : $"{CurrentUser.FirstName} {CurrentUser.LastName}";
            #endregion

            switch (ReportType)
            {
                case Constants.ReportType.SalesReport:
                    reportName = $"{Constants.ReportType.SalesReport} Report";
                    await PrintSalesReport(companyData!, fullName);
                    break;
                case Constants.ReportType.SalesSummaryReport:
                    reportName = $"{Constants.ReportType.SalesSummaryReport} Report";
                    await PrintSalesSummaryReport(companyData!, fullName);
                    break;
                case Constants.ReportType.CommissionsTech:
                    reportName = $"{Constants.ReportType.CommissionsTech} Report";
                    await PrintCommissionsTechReport(companyData!, fullName);
                    break;
                case Constants.ReportType.CommissionsSA:
                    reportName = $"{Constants.ReportType.CommissionsSA} Report";
                    await PrintCommissionsSAReport(companyData!, fullName);
                    break;
                case Constants.ReportType.IncentivesTech:
                    reportName = $"{Constants.ReportType.IncentivesTech} Report";
                    await PrintIncentivesTechReport(companyData!, fullName);
                    break;
                case Constants.ReportType.IncentivesSA:
                    reportName = $"{Constants.ReportType.IncentivesSA} Report";
                    await PrintIncentivesSAReport(companyData!, fullName);
                    break;
                case Constants.ReportType.Customers:
                    var customerData = await CustomerService.GetAllAsync();

                    if (customerData == null)
                        return;

                    CustomerListReportGenerator.ImageFile = FileHelper.GetRapideLogo();
                    CustomerListReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();

                    await CustomerListReportGenerator.Generate(customerData, JSRuntime, DateTime.Now, DateTime.Now.AddDays(5), companyData);
                    break;
                case Constants.ReportType.Vehicles:
                    reportName = $"{Constants.ReportType.Vehicles} Report";
                    break;
                case Constants.ReportType.Expenses:
                    reportName = $"{Constants.ReportType.Expenses} Report";
                    break;
                case Constants.ReportType.CreditCardPayment:
                    reportName = "Credit Card Payment Report";
                    await PrintCreditCardPaymentReport(companyData!, fullName);
                    break;
            }

            IsLoading = false;

        }
    }
}
