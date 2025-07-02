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

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class PaymentDetails
    {
        #region Parameters
        [Parameter]
        public string? PaymentId { get; set; }
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
        private IJobOrderService JobOrderService { get; set; }
        [Inject]
        private IJobOrderProductService JobOrderProductService { get; set; }
        [Inject]
        private IJobOrderServiceService JobOrderServiceService { get; set; }
        [Inject]
        private IJobOrderTechnicianService JobOrderTechnicianService { get; set; }
        [Inject]
        private IJobStatusService JobStatusService { get; set; }
        [Inject]
        private ICustomerService CustomerService { get; set; }
        [Inject]
        private IParameterService ParameterService { get; set; }
        [Inject]
        private IInvoiceService InvoiceService { get; set; }
        [Inject]
        private IInvoicePackageService InvoicePackageService { get; set; }
        [Inject]
        private IPaymentService PaymentService { get; set; }
        [Inject]
        private IPaymentDetailsService PaymentDetailsService { get; set; }
        [Inject]
        private IDepositService DepositService { get; set; }
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

        private PaymentDTO PaymentRequestModel { get; set; } = new();

        private List<JobStatusDTO> JobStatusList { get; set; } = new();
        private List<CustomerDTO> CustomerList { get; set; } = new();
        private List<ParameterDTO> PaymentTypeList { get; set; } = new();

        private string JobStatusName = string.Empty;
        private string CustomerName = string.Empty;
        private bool isPaymentLocked = false;
        private bool isBigThreeRoles = false;
        private bool isViewOnly = false;

        // From child components
        public List<InvoiceDTO> Invoices { get; set; } = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            IsEditMode = !string.IsNullOrEmpty(PaymentId);
            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var paymentTypeList = await ParameterService.GetAllParameterAsync();

            JobStatusList = await JobStatusService.GetAllAsync();
            CustomerList = await CustomerService.GetAllAsync();
            PaymentTypeList = paymentTypeList.Where(x => x.ParameterGroup.Name.Equals(Constants.ParameterType.PaymentTypeParam)).ToList();

            if (IsEditMode)
            {
                PaymentRequestModel = await PaymentService.GetPaymentByIdAsync(int.Parse(PaymentId));

                var jobStatusOpen = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                var jobStatusConverted = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Converted)).FirstOrDefault();
                var invoiceData = await InvoiceService.GetAllInvoiceByCustomerIdAsync(PaymentRequestModel.Customer.Id);

                PaymentRequestModel.InvoiceList = invoiceData.Where(x => x.JobStatusId == jobStatusOpen.Id || x.JobStatusId == jobStatusConverted.Id).ToList();

                PaymentRequestModel.PaymentDetailsList = await PaymentDetailsService.GetAllPaymentDetailsByPaymentIdAsync(PaymentRequestModel.Id);

                if (PaymentRequestModel.PaymentDetailsList == null)
                    PaymentRequestModel.PaymentDetailsList = new List<PaymentDetailsDTO>();

                Invoices = PaymentRequestModel.InvoiceList == null
                      ? new List<InvoiceDTO>()
                      : PaymentRequestModel.InvoiceList;

                // creteria of locked based on status.
                isPaymentLocked =
                    PaymentRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Converted) ||
                    PaymentRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Completed) ||
                    PaymentRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Cancelled) ||
                    isViewOnly;

                form.Disabled = isPaymentLocked;
                IsEditMode = !isPaymentLocked;

                // Update footer figures
                var paidAmount = PaymentRequestModel.PaymentDetailsList.Sum(x => x.AmountPaid);
                var invoiceAmount = PaymentRequestModel.PaymentDetailsList.GroupBy(x => x.Invoice.Id).Select(x => x.First()).Sum(x => x.Invoice.TotalAmount);
                PaymentRequestModel.InvoiceTotalAmount = invoiceAmount;
                PaymentRequestModel.AmountPayable = invoiceAmount;
                PaymentRequestModel.Balance = invoiceAmount - paidAmount;
                PaymentRequestModel.TotalPaidAmount = paidAmount;
            }
            else
            {
                PaymentRequestModel.PaymentDetailsList = new List<PaymentDetailsDTO>();

                PaymentRequestModel.JobStatus = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                PaymentRequestModel.PaymentDate = DateTime.Now;

                PaymentRequestModel.ReferenceNo = await ReferenceNumberHelper.GetRNPayment(PaymentService);
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

                    var nonCashPayment = PaymentRequestModel.PaymentDetailsList.Where(x => x.PaymentTypeParameter.Name != "CASH");

                    if (nonCashPayment != null && nonCashPayment.Any())
                    {
                        var checkReferenceNo = nonCashPayment.Where(x => string.IsNullOrEmpty(x.PaymentReferenceNo));

                        if (checkReferenceNo == null && !nonCashPayment.Any())
                        {
                            mBoxCustomMessage = "Reference number is required for non-cash payment.";
                            await mboxError.ShowAsync();

                            return;
                        }
                    }

                    //if (PaymentRequestModel.PaymentDetailsList.Where(x => x.DepositAmount > 0).Any())
                    //{
                    //    var depositInfo = PaymentRequestModel.PaymentDetailsList.Where(x => x.DepositAmount > 0);
                    //    if (depositInfo != null && depositInfo.Any())
                    //    {
                    //        var joId = depositInfo.FirstOrDefault()!.Invoice.JobOrderId;
                    //        var depositData = await DepositService.GetAllDepositByJobOrderIdAsync(joId);

                    //        if (depositData != null && depositData.Any())
                    //        {
                    //            var depositToUpdate = depositData.FirstOrDefault();

                    //            var jobStatusCompleted = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Completed)).FirstOrDefault();

                    //            depositToUpdate.JobStatus = jobStatusCompleted;
                    //            depositToUpdate.JobStatusId = jobStatusCompleted.Id;

                    //            await DepositService.UpdateAsync(depositToUpdate);
                    //        }
                    //    }

                    //}

                    bool isEditMode = !string.IsNullOrEmpty(PaymentId);

                    ReloadPaymentRequestModel();

                    if (!isEditMode) // create mode
                    {
                        PaymentRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        PaymentRequestModel.CreatedDateTime = DateTime.Now;
                        PaymentRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        PaymentRequestModel.UpdatedDateTime = DateTime.Now;

                        // call create endpoint here...
                        var created = await PaymentService.CreateAsync(PaymentRequestModel);

                        // Save payment details
                        foreach (var t in PaymentRequestModel.PaymentDetailsList)
                        {
                            t.PaymentId = created.Id;
                            t.PaymentTypeParameterId = t.PaymentTypeParameter.Id;
                            t.InvoiceId = t.Invoice.Id;
                            //t.DepositAmount = t.DepositAmount;
                            t.IsDeposit = t.IsDeposit;
                            t.PaymentDate = t.PaymentDate;
                            t.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            t.CreatedDateTime = DateTime.Now;
                            t.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            t.UpdatedDateTime = DateTime.Now;

                            await PaymentDetailsService.CreateAsync(t);
                        }

                        PaymentId = created.Id.ToString();
                        await ReloadPaymentData();

                        IsLoading = false;

                        SnackbarService.Add("Payment Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/payments", true);
                    }
                    else // update mode
                    {
                        await UpdatePaymentDetails();

                        SnackbarService.Add("Payment Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/payments");
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

        private async Task UpdatePaymentDetails()
        {
            int paymentId = int.Parse(PaymentId);

            PaymentRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
            PaymentRequestModel.UpdatedDateTime = DateTime.Now;

            // close the invoice if checkbox is checked.
            var completedInvoice = PaymentRequestModel.PaymentDetailsList.Where(x => x.IsFullyPaid == true).ToList();
            if (completedInvoice != null && completedInvoice.Any())
            {
                foreach (var i in completedInvoice)
                {
                    var amountPaid = completedInvoice.Sum(x => x.AmountPaid);
                    var invoiceAmount = i.Invoice.TotalAmount;

                    if (amountPaid >= invoiceAmount)
                    {
                        // message box here to confirm closing of invoice number
                        var jobStatusCompleted = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Completed)).FirstOrDefault();

                        // Validate if the invoice is paid or not before closing

                        i.Invoice.JobStatus = jobStatusCompleted;
                        i.Invoice.JobStatusId = jobStatusCompleted.Id;

                        await InvoiceService.UpdateAsync(i.Invoice);
                    }
                }
            }


            // call update endpoint here...
            await PaymentService.UpdateAsync(PaymentRequestModel);

            // Detele all current services? Update also include insert inside
            var paymentDetailsDelete = await PaymentDetailsService.GetAllPaymentDetailsByPaymentIdAsync(paymentId);

            if (paymentDetailsDelete != null && paymentDetailsDelete.Any())
            {
                foreach (var del in paymentDetailsDelete)
                {
                    await PaymentDetailsService.DeleteAsync(del.Id);
                }
            }

            foreach (var p in PaymentRequestModel.PaymentDetailsList)
            {
                p.Id = 0;
                p.PaymentId = paymentId;
                p.PaymentTypeParameterId = p.PaymentTypeParameter.Id;
                p.InvoiceId = p.Invoice.Id;
                p.IsFullyPaid = p.IsFullyPaid;
                //p.DepositAmount = p.DepositAmount;
                p.IsDeposit = p.IsDeposit;
                p.PaymentDate = p.PaymentDate;

                p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                p.CreatedDateTime = DateTime.Now;
                p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                p.UpdatedDateTime = DateTime.Now;

                await PaymentDetailsService.CreateAsync(p);
            }
        }

        private void ReloadPaymentRequestModel()
        {
            PaymentRequestModel.CustomerId = PaymentRequestModel.Customer.Id;
            PaymentRequestModel.JobStatusId = PaymentRequestModel.JobStatus.Id;
            PaymentRequestModel.InvoiceList = Invoices;
        }

        private async Task OnReOpenClick()
        {
            if (string.IsNullOrEmpty(PaymentId))
                return;

            bool? result = await mbox.ShowAsync();
            var proceed = result == null ? false : true;

            if (proceed)
            {
                IsLoading = true;

                var jobStatusOpen = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                PaymentRequestModel.JobStatus = jobStatusOpen;
                PaymentRequestModel.JobStatusId = jobStatusOpen.Id;

                await PaymentService.UpdateAsync(PaymentRequestModel);
                SnackbarService.Add("Payment Successfuly re-OPENED!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                IsLoading = false;
                StateHasChanged();

                NavigationManager.NavigateToCustom("/operations/payments", true);
            }
        }

        private async Task ReloadPaymentData()
        {
            // Get data by id
            PaymentRequestModel = await PaymentService.GetPaymentByIdAsync(int.Parse(PaymentId));
            JobStatusName = PaymentRequestModel.JobStatus.Name;

            PaymentRequestModel.PaymentDetailsList = await PaymentDetailsService.GetAllPaymentDetailsByPaymentIdAsync(PaymentRequestModel.Id);
        }

        private async Task OnCancelClick()
        {
            NavigationManager.NavigateToCustom("/operations/payments");
        }

        private async Task OnPaymentCompletedClick()
        {
            var isValidated = await ValidateSubComponents();

            if (!isValidated)
                return;

            if (PaymentRequestModel.Balance > 0)
            {
                mBoxCustomMessage = "Closing of payment is not allowed if balance is not yet fully paid!";
                await mboxError.ShowAsync();

                return;
            }

            // Validate if the invoice is paid or not before closing

            var completedInvoice = PaymentRequestModel.PaymentDetailsList.Where(x => x.IsFullyPaid == false).ToList();
            if (completedInvoice != null && completedInvoice.Any())
            {

                mBoxCustomMessage = "Closing of payment is not allowed if invoice is still open.!";
                await mboxError.ShowAsync();

                return;
            }

            mBoxCustomMessage = "Are you sure you want to complete this invoice and payment transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                try
                {
                    ReloadPaymentRequestModel();
                    await UpdatePaymentDetails();

                    var jobStatusCompleted = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Completed)).FirstOrDefault();

                    PaymentRequestModel.JobStatus = jobStatusCompleted;
                    PaymentRequestModel.JobStatusId = jobStatusCompleted.Id;

                    await PaymentService.UpdateAsync(PaymentRequestModel);

                    

                    SnackbarService.Add("Payment and Invoice Successfuly Completed!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                    NavigationManager.NavigateToCustom("/operations/payments", true);
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

        private async Task OnNewPaymentClick()
        { 
        }

        private async Task OnCancelPaymentClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the this transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                var jobStatusCancelled = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Cancelled)).FirstOrDefault();
                PaymentRequestModel.JobStatus = jobStatusCancelled;
                PaymentRequestModel.JobStatusId = jobStatusCancelled.Id;

                await PaymentService.UpdateAsync(PaymentRequestModel);

                SnackbarService.Add("Payment Successfuly Cancelled!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                NavigationManager.NavigateToCustom("/operations/payments", true);
            }
        }

        private void PaymentDetailsItemHasChanged(List<PaymentDetailsDTO> e)
        {
            PaymentRequestModel.PaymentDetailsList = e;
            PaymentRequestModel.DepositAmount = e.Where(x => x.IsDeposit == true).Sum(x => x.AmountPaid);

            var invoiceAmount = e.GroupBy(x => x.Invoice.Id).Select(x => x.First()).Sum(x => x.Invoice.TotalAmount);
            PaymentRequestModel.InvoiceTotalAmount = invoiceAmount;
            PaymentRequestModel.AmountPayable = invoiceAmount;

            var totalAmountPaid = e.Where(x => x.IsDeposit == false).Sum(x => x.AmountPaid);
            PaymentRequestModel.TotalPaidAmount = totalAmountPaid + PaymentRequestModel.DepositAmount;

            PaymentRequestModel.Balance = invoiceAmount - totalAmountPaid - PaymentRequestModel.DepositAmount;

            StateHasChanged();
        }

        private async Task OnCustomerChanged(PaymentDTO dto, CustomerDTO i)
        {
            PaymentRequestModel.PaymentDetailsList = new List<PaymentDetailsDTO>();

            var jobStatusOpen = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
            var jobStatusConverted = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Converted)).FirstOrDefault();

            var invoiceData = await InvoiceService.GetAllInvoiceByCustomerIdAsync(i.Id);
            Invoices = invoiceData.Where(x => x.JobStatusId == jobStatusOpen.Id || x.JobStatusId == jobStatusConverted.Id).ToList();

            PaymentRequestModel.Customer = i;
            PaymentRequestModel.InvoiceList = Invoices;

            var depositInfo = await DepositService.GetAllDepositByCustomerIdAsync(i.Id);
            PaymentRequestModel.DepositAmount = 0;
            if (depositInfo != null)
            {
                PaymentRequestModel.DepositAmount = depositInfo.Sum(x => x.DepositAmount);

                foreach (var di in depositInfo)
                {
                    PaymentRequestModel.PaymentDetailsList.Add(
                        new PaymentDetailsDTO()
                        {
                            Payment = PaymentRequestModel,
                            PaymentId = PaymentRequestModel.Id,
                            PaymentTypeParameter = di.PaymentTypeParameter,
                            PaymentTypeParameterId = di.PaymentTypeParameterId,
                            Invoice = PaymentRequestModel.InvoiceList.FirstOrDefault(),
                            InvoiceId = PaymentRequestModel.InvoiceList.FirstOrDefault().Id,
                            IsFullyPaid = true,
                            IsDeposit = true,
                            PaymentDate = di.TransactionDateTime.Value,
                            //DepositAmount = 0,
                            AmountPaid = di.DepositAmount,
                            PaymentReferenceNo = di.PaymentReferenceNo
                        });
                }
            }
            

            var invoiceTotalAmount = Invoices == null 
                ? 0 
                : Invoices.Sum(x => x.TotalAmount);

            PaymentRequestModel.InvoiceTotalAmount = invoiceTotalAmount;

            PaymentRequestModel.VAT12 = invoiceTotalAmount == 0 ? 0 : invoiceTotalAmount * 12 / 100;

            PaymentRequestModel.AmountPayable = invoiceTotalAmount - PaymentRequestModel.DepositAmount;

            StateHasChanged();
        }

        private void OnAddNewItemClick(DialogTypes dialogType)
        {
            bool isEditMode = !string.IsNullOrEmpty(PaymentId);
            var returnUrl = isEditMode
                ? $"/operations/payments/{PaymentId}"
                : "/operations/payments/add";

            if (dialogType == DialogTypes.Customer)
                NavigationManager.NavigateToCustom($"/customers/add?returnUrl={returnUrl}");
        }

        private async Task<bool> ValidateSubComponents()
        {
            if (!PaymentRequestModel.PaymentDetailsList.Any())
            {
                mBoxCustomMessage = "Payment details is required!";
                await mboxError.ShowAsync();

                return false;
            }

            if (PaymentRequestModel.PaymentDetailsList.Where(x => x.AmountPaid.Equals(0)).Any())
            {
                mBoxCustomMessage = "No selected payment or payment is empty!";
                await mboxError.ShowAsync();

                return false;
            }

            return true;
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
        #endregion

        private async Task OnGeneratePdfClick()
        {
            IsLoading = true;
            int paymentId = int.Parse(PaymentId);

            var PaymentRequestModel = await PaymentService.GetPaymentByIdAsync(paymentId);
            var paymentDetailsInfo = await PaymentDetailsService.GetAllPaymentDetailsByPaymentIdAsync(paymentId);

            if (paymentDetailsInfo == null)
            {
                IsLoading = false;
                StateHasChanged();

                mBoxCustomMessage = "Payment details is empty!";
                await mboxError.ShowAsync();

                return;
            }

            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            var InvoiceRequestModel = await InvoiceService.GetInvoiceByIdAsync(paymentDetailsInfo.FirstOrDefault().InvoiceId);
            var invoiceInfo = InvoiceRequestModel;

            invoiceInfo.PackageList = await InvoicePackageService.GetAllInvoicePackageByInvoiceIdAsync(invoiceInfo.Id);
            invoiceInfo.ProductList = await JobOrderProductService.GetAllJobOrderProductByJobOrderIdAsync(invoiceInfo.JobOrder.Id);
            invoiceInfo.ServiceList = await JobOrderServiceService.GetAllJobOrderServiceByJobOrderIdAsync(invoiceInfo.JobOrder.Id);
            invoiceInfo.TechnicianList = await JobOrderTechnicianService.GetAllJobOrderTechnicianByJobOrderIdAsync(invoiceInfo.JobOrder.Id);

            // payment information

            InvoiceReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            InvoiceReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
            await InvoiceReportGenerator.Generate(invoiceInfo, JSRuntime, PaymentRequestModel, companyData);

            IsLoading = false;
        }

        private async Task OnGenerateGatePassPdfClick()
        {
            IsLoading = true;

            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            var JobOrderRequestModel = await JobOrderService.GetAllJobOrderByCustomerIdAsync(PaymentRequestModel.Customer.Id);

            if (JobOrderRequestModel == null || !JobOrderRequestModel.Any())
            {
                return;
            }

            GatePassReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            GatePassReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
            await GatePassReportGenerator.Generate(JobOrderRequestModel.FirstOrDefault(), JSRuntime, companyData);

            IsLoading = false;
        }
    }
}