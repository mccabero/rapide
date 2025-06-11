using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.PdfReportGenerator;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class QuickSalesDetails
    {
        #region Parameters
        [Parameter]
        public string? QuickSalesId { get; set; }
        #endregion

        #region Dependency Injection
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private IJobStatusService JobStatusService { get; set; }
        [Inject]
        private ICustomerService CustomerService { get; set; }
        [Inject]
        private IParameterService ParameterService { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IQuickSalesService QuickSalesService { get; set; }
        [Inject]
        private IQuickSalesProductService QuickSalesProductService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        #endregion

        #region Private Properties
        private MudForm form;
        private string[] errors = { };
        private bool success;

        private MudMessageBox mboxCustom { get; set; }
        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }
        private bool IsEditMode { get; set; }

        private QuickSalesDTO QuickSalesRequestModel { get; set; } = new();

        private List<JobStatusDTO> JobStatusList { get; set; } = new();
        private List<CustomerDTO> CustomerList { get; set; } = new();
        private List<ParameterDTO> PaymentTypeList { get; set; } = new();
        private List<UserDTO> SalesPersonUserList { get; set; } = new();

        private string JobStatusName = string.Empty;
        private string CustomerName = string.Empty;
        private bool isQuickSalesLocked = false;
        private bool isBigThreeRoles = false;
        private bool isViewOnly = false;

        // From child components
        public List<QuickSalesProductDTO> QuickSalesProducts { get; set; } = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            IsEditMode = !string.IsNullOrEmpty(QuickSalesId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);

            var paymentTypeList = await ParameterService.GetAllParameterAsync();
            var userList = await UserService.GetAllUserRoleAsync();

            JobStatusList = await JobStatusService.GetAllAsync();
            CustomerList = await CustomerService.GetAllAsync();
            PaymentTypeList = paymentTypeList.Where(x => x.ParameterGroup.Name.Equals(Constants.ParameterType.PaymentTypeParam)).ToList();
            SalesPersonUserList = userList;

            if (IsEditMode)
            {
                QuickSalesRequestModel = await QuickSalesService.GetQuickSalesByIdAsync(int.Parse(QuickSalesId));
                QuickSalesRequestModel.ProductList = await QuickSalesProductService.GetAllQuickSalesProductByQuickSalesIdAsync(QuickSalesRequestModel.Id);

                QuickSalesProducts = QuickSalesRequestModel.ProductList == null
                    ? new List<QuickSalesProductDTO>()
                    : QuickSalesRequestModel.ProductList;

                // creteria of locked based on status.
                isQuickSalesLocked =
                    QuickSalesRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Converted) ||
                    QuickSalesRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Completed) ||
                    QuickSalesRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Cancelled) ||
                    isViewOnly;

                form.Disabled = isQuickSalesLocked;
                IsEditMode = !isQuickSalesLocked;
            }
            else
            {
                QuickSalesRequestModel.JobStatus = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                QuickSalesRequestModel.TransactionDate = DateTime.Now;

                var existingQuickSales = await QuickSalesService.GetAllQuickSalesAsync();
                var lastId = 1;

                if (existingQuickSales != null)
                {
                    var lastRefNo = existingQuickSales.LastOrDefault().ReferenceNo.Replace("QS", string.Empty);

                    lastId = string.IsNullOrEmpty(lastRefNo) ? 0 : int.Parse(lastRefNo);
                    lastId++;
                }

                QuickSalesRequestModel.ReferenceNo = await ReferenceNumberHelper.GetRNQuickSales(QuickSalesService);
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task OnSaveClick()
        {
            await form.Validate();

            if (!form.IsValid)
                return;

            var isValidated = await ValidateSubComponents();

            if (!isValidated)
                return;

            bool? result = await mbox.ShowAsync();
            var proceedSaving = result == null ? false : true;

            if (proceedSaving)
            {
                try
                {
                    IsLoading = true;
                    bool isEditMode = !string.IsNullOrEmpty(QuickSalesId);

                    QuickSalesRequestModel.CustomerId = QuickSalesRequestModel.Customer.Id;
                    QuickSalesRequestModel.PaymentTypeParameterId = QuickSalesRequestModel.PaymentTypeParameter.Id;
                    QuickSalesRequestModel.SalesPersonUserId = QuickSalesRequestModel.SalesPersonUser.Id;
                    QuickSalesRequestModel.JobStatusId = QuickSalesRequestModel.JobStatus.Id;

                    QuickSalesRequestModel.ProductList = QuickSalesProducts;

                    if (!isEditMode) // create mode
                    {
                        QuickSalesRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        QuickSalesRequestModel.CreatedDateTime = DateTime.Now;
                        QuickSalesRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        QuickSalesRequestModel.UpdatedDateTime = DateTime.Now;

                        // call create endpoint here...
                        var created = await QuickSalesService.CreateAsync(QuickSalesRequestModel);

                        // Save products
                        foreach (var p in QuickSalesRequestModel.ProductList)
                        {
                            p.QuickSalesId = created.Id;
                            p.ProductId = p.Product.Id;

                            p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.CreatedDateTime = DateTime.Now;
                            p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.UpdatedDateTime = DateTime.Now;

                            await QuickSalesProductService.CreateAsync(p);
                        }

                        SnackbarService.Add("Quick Sales Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/quick-sales");
                    }
                    else
                    {
                        int quickSalesId = int.Parse(QuickSalesId);

                        QuickSalesRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        QuickSalesRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await QuickSalesService.UpdateAsync(QuickSalesRequestModel);

                        // Detele all current services? Update also include insert inside
                        var quickSalesProductList = await QuickSalesProductService.GetAllQuickSalesProductByQuickSalesIdAsync(quickSalesId);

                        if (quickSalesProductList != null && quickSalesProductList.Any())
                        {
                            foreach (var del in quickSalesProductList)
                            {
                                    await QuickSalesProductService.DeleteAsync(del.Id);
                            }
                        }

                        foreach (var p in QuickSalesRequestModel.ProductList)
                        {
                            p.Id = 0;
                            p.ProductId = p.Product.Id;
                            p.QuickSalesId = quickSalesId;
                            p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.CreatedDateTime = DateTime.Now;
                            p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.UpdatedDateTime = DateTime.Now;

                            await QuickSalesProductService.CreateAsync(p);
                        }

                        SnackbarService.Add("Quick Sales Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/quick-sales");
                    }

                }
                catch (Exception ex)
                {
                    SnackbarService.Add(
                        $"Error occurred while processing the transaction. Please contact your systems administrator.{Environment.NewLine}" +
                        $"Error Message: {ex.Message} ",
                        Severity.Error,
                        config => { config.ShowCloseIcon = true; });

                    IsLoading = false;
                }
            }
        }

        private async Task OnReOpenClick()
        {
            if (string.IsNullOrEmpty(QuickSalesId))
                return;

            bool? result = await mbox.ShowAsync();
            var proceed = result == null ? false : true;

            if (proceed)
            {
                IsLoading = true;

                var jobStatusOpen = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                QuickSalesRequestModel.JobStatus = jobStatusOpen;
                QuickSalesRequestModel.JobStatusId = jobStatusOpen.Id;

                await QuickSalesService.UpdateAsync(QuickSalesRequestModel);
                SnackbarService.Add("Quick Sales Successfuly re-OPENED!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                IsLoading = false;
                StateHasChanged();

                NavigationManager.NavigateToCustom("/operations/quick-sales", true);
            }
        }

        private async Task OnCancelClick()
        {
            NavigationManager.NavigateToCustom("/operations/quick-sales");
        }

        private async Task OnQuickSalesCompletedClick()
        {
            mBoxCustomMessage = "Are you sure you want to move this estimate to job order?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                var jobStatus = await JobStatusService.GetAllAsync();

                try
                {
                    var convertedStatus = jobStatus.Where(x => x.Name.Equals(Constants.JobStatus.Completed)).FirstOrDefault();
                    QuickSalesRequestModel.JobStatus = convertedStatus;
                    QuickSalesRequestModel.JobStatusId = convertedStatus.Id;

                    await QuickSalesService.UpdateAsync(QuickSalesRequestModel);

                    NavigationManager.NavigateTo($"/operations/quick-sales");

                }
                catch (Exception ex)
                {
                    SnackbarService.Add(
                        $"Error occurred while processing the transaction. Please contact your systems administrator.{Environment.NewLine}" +
                        $"Error Message: {ex.Message} ",
                        Severity.Error,
                        config => { config.ShowCloseIcon = true; });

                    IsLoading = false;
                }
            }
        }

        private async Task OnNewQuickSalesClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the current transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
                NavigationManager.NavigateToCustom("/operations/quick-sales/add", true);
        }

        private async Task OnCancelQuickSalesClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the this transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                var jobStatusCancelled = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Cancelled)).FirstOrDefault();
                QuickSalesRequestModel.JobStatus = jobStatusCancelled;
                QuickSalesRequestModel.JobStatusId = jobStatusCancelled.Id;

                await QuickSalesService.UpdateAsync(QuickSalesRequestModel);

                SnackbarService.Add("Quick Sales Successfuly Cancelled!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                NavigationManager.NavigateToCustom("/operations/quick-sales", true);
            }
        }

        private void OnSubTotalChanged(QuickSalesDTO dto, string i)
        {
            QuickSalesRequestModel.VAT12 = decimal.Parse(i) * 12 / 100;

            QuickSalesRequestModel.TotalAmount = QuickSalesRequestModel.SubTotal - QuickSalesRequestModel.Discount;
            StateHasChanged();
        }

        private void OnDiscountChanged(QuickSalesDTO dto, decimal i)
        {
            if (i > QuickSalesRequestModel.SubTotal)
                return;

            QuickSalesRequestModel.Discount = i;

            QuickSalesRequestModel.TotalAmount = QuickSalesRequestModel.SubTotal - QuickSalesRequestModel.Discount;
            StateHasChanged();
        }

        private void OnPaymentChanged(QuickSalesDTO dto, decimal i)
        {
            QuickSalesRequestModel.Payment = i;

            QuickSalesRequestModel.Change = QuickSalesRequestModel.TotalAmount - QuickSalesRequestModel.Payment;
            StateHasChanged();
        }

        private void ProductItemHasChanged(List<QuickSalesProductDTO> e)
        {
            QuickSalesProducts = e;
            var productTotal = QuickSalesProducts == null
                ? 0
                : QuickSalesProducts.Sum(x => x.Amount);

            QuickSalesRequestModel.SubTotal = productTotal;

            StateHasChanged();
        }

        private async Task OnCustomerChanged(QuickSalesDTO dto, CustomerDTO i)
        {
            QuickSalesRequestModel.Customer = i;

            StateHasChanged();
        }

        private void OnAddNewItemClick(DialogTypes dialogType)
        {
            bool isEditMode = !string.IsNullOrEmpty(QuickSalesId);
            var returnUrl = isEditMode
                ? $"/operations/quick-sales/{QuickSalesId}"
                : "/operations/quick-sales/add";

            //if (dialogType == EstimateDialogType.Customer)
            //    NavigationManager.NavigateToCustom($"/customers/add?returnUrl={returnUrl}");
            //else if (dialogType == EstimateDialogType.Vehicle)
            //    NavigationManager.NavigateToCustom($"/vehicles/add?returnUrl={returnUrl}");
            //else if (dialogType == EstimateDialogType.User)
            //    NavigationManager.NavigateToCustom($"/administrators/users/add?returnUrl={returnUrl}");
            //else if (dialogType == EstimateDialogType.ServiceGroup)
            //    NavigationManager.NavigateToCustom($"/configurations/service-groups/add?returnUrl={returnUrl}");
            //else if (dialogType == EstimateDialogType.JobStatus)
            //    NavigationManager.NavigateToCustom($"/configurations/job-statuses/add?returnUrl={returnUrl}");
        }

        #region Search MudAutoComplete
        private async Task<IEnumerable<JobStatusDTO>> SearchJobStatus(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return JobStatusList;

            return JobStatusList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<CustomerDTO>> SearchCustomer(string filter, CancellationToken token)
        {
            CustomerList = CustomerList.OrderByDescending(x => x.CreatedDateTime).ToList();

            if (string.IsNullOrEmpty(filter))
                return CustomerList;

            return CustomerList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ParameterDTO>> SearchPaymentType(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return PaymentTypeList;

            return PaymentTypeList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<UserDTO>> SearchSalesPersonUser(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return SalesPersonUserList;

            return SalesPersonUserList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
        #endregion

        private async Task<bool> ValidateSubComponents()
        {
            if (!QuickSalesProducts.Any())
            {
                mBoxCustomMessage = "Products is required!";
                await mboxError.ShowAsync();

                return false;
            }

            if (QuickSalesProducts.Where(x => x.Product.Id.Equals(0)).Any())
            {
                mBoxCustomMessage = "No selected product or product name is empty!";
                await mboxError.ShowAsync();

                return false;
            }

            return true;
        }

        private async Task OnGeneratePdfClick()
        {
            var quickSalesId = int.Parse(QuickSalesId);

            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            var QuickSalesRequestModel = await QuickSalesService.GetQuickSalesByIdAsync(quickSalesId);
            QuickSalesRequestModel.ProductList = await QuickSalesProductService.GetAllQuickSalesProductByQuickSalesIdAsync(quickSalesId);

            if (QuickSalesRequestModel == null)
                return;

            QuickSalesReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            QuickSalesReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
            await QuickSalesReportGenerator.Generate(QuickSalesRequestModel, JSRuntime, companyData);
        }
    }
}