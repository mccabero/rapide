
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
    public partial class InvoiceDetails
    {
        #region Parameters
        [Parameter]
        public string? InvoiceId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private IInvoiceService InvoiceService { get; set; }
        [Inject]
        private IJobOrderServiceService JobOrderServiceService { get; set; }
        [Inject]
        private IJobOrderProductService JobOrderProductService { get; set; }
        [Inject]
        private IJobOrderTechnicianService JobOrderTechnicianService { get; set; }
        [Inject]
        private IJobStatusService JobStatusService { get; set; }
        [Inject]
        private ICustomerService CustomerService { get; set; }
        [Inject]
        private IVehicleService VehicleService { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IServiceGroupService ServiceGroupService { get; set; }
        [Inject]
        private IPackageService PackageService { get; set; }
        [Inject]
        private IInvoicePackageService InvoicePackageService { get; set; }
        [Inject]
        private IPaymentService PaymentService { get; set; }
        [Inject]
        private IPaymentDetailsService PaymentDetailsService { get; set; }
        [Inject]
        private IDepositService DepositService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private InvoiceDTO InvoiceRequestModel { get; set; } = new();

        private List<JobStatusDTO> JobStatusList { get; set; } = new();
        private List<CustomerDTO> CustomerList { get; set; } = new();
        private List<VehicleDTO> VehicleList { get; set; } = new();
        private List<UserDTO> ServiceAdvisorList { get; set; } = new();
        private List<UserDTO> EstimatorList { get; set; } = new();
        private List<UserDTO> ApproverList { get; set; } = new();
        private List<ServiceGroupDTO> ServiceGroupList { get; set; } = new();

        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxCustom { get; set; }
        private MudMessageBox mboxError { get; set; }
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }
        private bool IsEditMode { get; set; }
        private bool isBigThreeRoles = false;

        // From child components
        public List<JobOrderServiceDTO> JobOrderServices { get; set; } = new();
        public List<JobOrderProductDTO> JobOrderProducts { get; set; } = new();
        public List<JobOrderTechnicianDTO> JobOrderTechnicians { get; set; } = new();
        private List<InvoicePackageDTO> InvoicePackages { get; set; } = new();

        private string JobStatusName = string.Empty;
        private string CustomerName = string.Empty;

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isJobOrderLocked = true;

        private bool IsPackage = false;
        private string PackageName = string.Empty;
        private bool hasPayment = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            ReloadInvoiceRequestModel();

            IsEditMode = !string.IsNullOrEmpty(InvoiceId);
            form = new MudForm();
            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);

            JobStatusList = await JobStatusService.GetAllAsync();

            if (IsEditMode)
            {
                await ReloadInvoiceData();

                // creteria of locked based on status.
                isJobOrderLocked =
                    InvoiceRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Converted) ||
                    InvoiceRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Completed) ||
                    InvoiceRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Cancelled);

                form.Disabled = isJobOrderLocked;
                IsEditMode = !isJobOrderLocked;
            }

            isJobOrderLocked = true; // invoice is always disabled
            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task OnProceedToPaymentClick()
        {
            var PaymentRequestModel = new PaymentDTO();

            mBoxCustomMessage = "Are you sure you want to move this invoice to payment?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                PaymentRequestModel.ReferenceNo = await ReferenceNumberHelper.GetRNPayment(PaymentService);

                var jobStatus = await JobStatusService.GetAllAsync();
                var newJobStatus = jobStatus.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();

                PaymentRequestModel.PaymentDate = DateTime.Now;

                var invoiceList = await InvoiceService.GetAllInvoiceByCustomerIdAsync(InvoiceRequestModel.Customer.Id);
                var invoicePayable = invoiceList.Any() && invoiceList != null
                    ? invoiceList.Sum(x => x.TotalAmount)
                    : 0;

                PaymentRequestModel.InvoiceTotalAmount = invoicePayable;
                PaymentRequestModel.AmountPayable = invoicePayable;

                PaymentRequestModel.CustomerId = InvoiceRequestModel.Customer.Id;
                PaymentRequestModel.JobStatusId = newJobStatus.Id;

                PaymentRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                PaymentRequestModel.CreatedDateTime = DateTime.Now;
                PaymentRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                PaymentRequestModel.UpdatedDateTime = DateTime.Now;

                // call create endpoint here...
                var created = await PaymentService.CreateAsync(PaymentRequestModel);

                // update the status to converted
                var convertedStatus = jobStatus.Where(x => x.Name.Equals(Constants.JobStatus.Converted)).FirstOrDefault();

                InvoiceRequestModel.JobStatus = convertedStatus;
                InvoiceRequestModel.JobStatusId = convertedStatus.Id;

                await InvoiceService.UpdateAsync(InvoiceRequestModel);

                //// Save payment details
                //foreach (var t in PaymentRequestModel.PaymentDetailsList)
                //{
                //    t.PaymentId = created.Id;
                //    t.PaymentTypeParameterId = t.PaymentTypeParameter.Id;
                //    t.InvoiceId = t.Invoice.Id;

                //    t.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                //    t.CreatedDateTime = DateTime.Now;
                //    t.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                //    t.UpdatedDateTime = DateTime.Now;

                //    await PaymentDetailsService.CreateAsync(t);
                //}

                IsLoading = false;
                ReloadInvoiceRequestModel();

                SnackbarService.Add("Payment Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                NavigationManager.NavigateToCustom($"/operations/payments/{created.Id}", true);
            }
        }

        private void ReloadInvoiceRequestModel()
        {
            InvoiceRequestModel = new InvoiceDTO()
            {
                Customer = new CustomerDTO(),
                AdvisorUser = new UserDTO(),
                JobOrder = new JobOrderDTO()
                {
                    Customer = new CustomerDTO(),
                    AdvisorUser = new UserDTO(),
                    ApproverUser = new UserDTO(),
                    Estimate = new EstimateDTO(),
                    EstimatorUser = new UserDTO(),
                    JobStatus = new JobStatusDTO(),
                    Vehicle = new VehicleDTO()
                    {
                        VehicleModel = new VehicleModelDTO()
                        {
                            VehicleMake = new VehicleMakeDTO()
                        }
                    }
                }
            };
        }

        private async Task OnSaveClick()
        {
            
        }

        private async Task OnReOpenClick()
        {
            if (string.IsNullOrEmpty(InvoiceId))
                return;

            bool? result = await mbox.ShowAsync();
            var proceed = result == null ? false : true;

            if (proceed)
            {
                IsLoading = true;

                var jobStatusOpen = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                InvoiceRequestModel.JobStatus = jobStatusOpen;
                InvoiceRequestModel.JobStatusId = jobStatusOpen.Id;

                await InvoiceService.UpdateAsync(InvoiceRequestModel);
                SnackbarService.Add("Invoice Successfuly re-OPENED!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                // object ref issue.
                InvoiceRequestModel.JobStatus = new JobStatusDTO();
                InvoiceRequestModel.JobOrder = new JobOrderDTO();
                InvoiceRequestModel.Customer = new CustomerDTO();

                IsLoading = false;
                StateHasChanged();

                NavigationManager.NavigateToCustom("/operations/invoices", true);
            }
        }

        private async Task OnCancelClick()
        {
            NavigationManager.NavigateToCustom("/operations/invoices");
        }

        private async Task OnCancelInvoiceClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the this transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                var jobStatusCancelled = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Cancelled)).FirstOrDefault();
                InvoiceRequestModel.JobStatus = jobStatusCancelled;
                InvoiceRequestModel.JobStatusId = jobStatusCancelled.Id;

                await InvoiceService.UpdateAsync(InvoiceRequestModel);

                SnackbarService.Add("Invoice Successfuly Cancelled!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                NavigationManager.NavigateToCustom("/operations/invoices", true);
            }
        }

        private async Task ReloadInvoiceData()
        {
            var invoiceId = int.Parse(InvoiceId);

            var paymentInfo = await PaymentDetailsService.GetAllPaymentDetailsByInvoiceIdAsync(invoiceId);
            if (paymentInfo != null && paymentInfo.Any())
                hasPayment = true;

            InvoiceRequestModel = await InvoiceService.GetInvoiceByIdAsync(invoiceId);
            JobStatusName = InvoiceRequestModel.JobStatus.Name;
            CustomerName = $"{InvoiceRequestModel.Customer.FirstName} {InvoiceRequestModel.Customer.LastName}";

            // Package data
            IsPackage = InvoiceRequestModel.IsPackage;
            if (IsPackage)
            {
                var packageListInfo = await InvoicePackageService.GetAllInvoicePackageByInvoiceIdAsync(InvoiceRequestModel.Id);
                InvoicePackages = packageListInfo;
            }

            InvoiceRequestModel.ProductList = await JobOrderProductService.GetAllJobOrderProductByJobOrderIdAsync(InvoiceRequestModel.JobOrder.Id);
            InvoiceRequestModel.ServiceList = await JobOrderServiceService.GetAllJobOrderServiceByJobOrderIdAsync(InvoiceRequestModel.JobOrder.Id);
            InvoiceRequestModel.TechnicianList = await JobOrderTechnicianService.GetAllJobOrderTechnicianByJobOrderIdAsync(InvoiceRequestModel.JobOrder.Id);
            InvoiceRequestModel.PackageList = InvoicePackages;

            JobOrderServices = InvoiceRequestModel.ServiceList == null
                ? new List<JobOrderServiceDTO>()
                : InvoiceRequestModel.ServiceList;

            JobOrderProducts = InvoiceRequestModel.ProductList == null
                ? new List<JobOrderProductDTO>()
                : InvoiceRequestModel.ProductList;

            JobOrderTechnicians = InvoiceRequestModel.TechnicianList == null
                ? new List<JobOrderTechnicianDTO>()
                : InvoiceRequestModel.TechnicianList;
        }

        private async Task OnGeneratePdfClick()
        {
            //IsLoading = true;

            //InvoiceReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            //await InvoiceReportGenerator.Generate(InvoiceRequestModel, JSRuntime);

            //IsLoading = false;
        }
    }
}