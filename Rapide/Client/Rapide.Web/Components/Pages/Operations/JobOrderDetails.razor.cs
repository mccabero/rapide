using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Services;
using Rapide.Web.Components.Pages.Components;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.PdfReportGenerator;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class JobOrderDetails
    {
        #region Parameters
        [Parameter]
        public string? JobOrderId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private IInvoiceService InvoiceService { get; set; }
        [Inject]
        private IJobOrderService JobOrderService { get; set; }
        [Inject]
        private IJobOrderServiceService JobOrderServiceService { get; set; }
        [Inject]
        private IJobOrderProductService JobOrderProductService { get; set; }
        [Inject]
        private IJobOrderPackageService JobOrderPackageService { get; set; }
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
        private IUserRolesService UserRolesService { get; set; }
        [Inject]
        private IServiceGroupService ServiceGroupService { get; set; }
        [Inject]
        private IPackageService PackageService { get; set; }
        [Inject]
        private IPackageProductService PackageProductService { get; set; }
        [Inject]
        private IPackageServiceService PackageServiceService { get; set; }
        [Inject]
        private IInvoicePackageService InvoicePackageService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IDialogService DialogService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        #endregion

        #region Private Properties
        private JobOrderDTO JobOrderRequestModel { get; set; } = new();

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


        // From child components
        public List<JobOrderServiceDTO> JobOrderServices { get; set; } = new();
        public List<JobOrderProductDTO> JobOrderProducts { get; set; } = new();
        public List<JobOrderTechnicianDTO> JobOrderTechnicians { get; set; } = new();
        private List<JobOrderPackageDTO> JobOrderPackages { get; set; } = new();

        private string JobStatusName = string.Empty;
        private string CustomerName = string.Empty;

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isJobOrderLocked = true;
        private bool isJobOrderCompleted = false;

        private bool IsPackage = false;
        private string PackageName = string.Empty;

        private bool isBigThreeRoles = false;
        private bool isViewOnly = false;
        private bool isCashier = false;
        private bool isCompleteJoDisabled = false;
        private bool isOIC = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isCashier = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier);
            isOIC = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.OIC);

            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.ServiceAdvisor)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var userList = await UserService.GetAllUserRoleAsync();
            var userWithRoles = new List<UserDTO>();

            foreach (var u in userList)
            {
                var userInfo = new UserDTO();

                userInfo = u;
                userInfo.UserRoles = await UserRolesService.GetUserRolesByUserIdAsync(u.Id);
            }

            IsEditMode = !string.IsNullOrEmpty(JobOrderId);
            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);

            CustomerList = await CustomerService.GetAllAsync();
            ServiceAdvisorList = userList.Where(x => x.IsActive && x.UserRoles.Any(x => x.Role.Name.Equals(Constants.UserRoles.ServiceAdvisor))).ToList();
            EstimatorList = userList.Where(x => x.IsActive && x.UserRoles.Any(x => x.Role.Name.Equals(Constants.UserRoles.Estimator))).ToList();
            ApproverList = userList.Where(x => x.IsActive && x.UserRoles.Any(x => x.Role.Name.Equals(Constants.UserRoles.Supervisor))).ToList();
            ServiceGroupList = await ServiceGroupService.GetAllAsync();
            JobStatusList = await JobStatusService.GetAllAsync();

            if (IsEditMode)
            {
                await ReloadJobOrderData();
                CustomerName = $"{JobOrderRequestModel.Customer.FirstName} {JobOrderRequestModel.Customer.LastName}";

                if (isBigThreeRoles)
                    isViewOnly = false;

                // creteria of locked based on status.
                isJobOrderLocked =
                    JobOrderRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Converted) ||
                    JobOrderRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Completed) ||
                    JobOrderRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Cancelled);

                form.Disabled = isJobOrderLocked ||
                    isViewOnly || isCashier;

                IsEditMode = !isJobOrderLocked;
                isCompleteJoDisabled = JobOrderRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Converted) ||
                    JobOrderRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Completed) ||
                    JobOrderRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Cancelled) ||
                    isViewOnly;

                if (isCompleteJoDisabled)
                {
                    isCompleteJoDisabled = !isOIC;
                }
            }
            else
            {
                CustomerName = "New Job Order";
                JobOrderRequestModel.JobStatus = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                JobOrderRequestModel.TransactionDate = DateTime.Now;
                JobOrderRequestModel.ExpirationDate = DateTime.Now.AddMonths(1);
                JobStatusName = JobOrderRequestModel.JobStatus.Name;

                JobOrderRequestModel.ReferenceNo = await ReferenceNumberHelper.GetRNJobOrder(JobOrderService);

            }

            if (!JobOrderRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Open))
            {
                form.Disabled = true;
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task ReloadJobOrderData()
        {
            // Get data by id
            JobOrderRequestModel = await JobOrderService.GetJobOrderByIdAsync(int.Parse(JobOrderId));
            JobStatusName = JobOrderRequestModel.JobStatus.Name;

            VehicleList = await VehicleService.GetAllVehicleByCustomerIdAsync(JobOrderRequestModel.CustomerId);

            // Package data
            IsPackage = JobOrderRequestModel.IsPackage;
            if (IsPackage)
            {
                var packageInfo = await JobOrderPackageService.GetAllJobOrderPackageByJobOrderIdAsync(JobOrderRequestModel.Id);
                JobOrderPackages = packageInfo == null ? new List<JobOrderPackageDTO>() : packageInfo;
            }

            JobOrderRequestModel.ProductList = await JobOrderProductService.GetAllJobOrderProductByJobOrderIdAsync(JobOrderRequestModel.Id);
            JobOrderRequestModel.ServiceList = await JobOrderServiceService.GetAllJobOrderServiceByJobOrderIdAsync(JobOrderRequestModel.Id);
            JobOrderRequestModel.TechnicianList = await JobOrderTechnicianService.GetAllJobOrderTechnicianByJobOrderIdAsync(JobOrderRequestModel.Id);
            JobOrderRequestModel.PackageList = JobOrderPackages;

            JobOrderServices = JobOrderRequestModel.ServiceList == null
                ? new List<JobOrderServiceDTO>()
                : JobOrderRequestModel.ServiceList;

            JobOrderProducts = JobOrderRequestModel.ProductList == null
                ? new List<JobOrderProductDTO>()
                : JobOrderRequestModel.ProductList;

            JobOrderTechnicians = JobOrderRequestModel.TechnicianList == null
                ? new List<JobOrderTechnicianDTO>()
                : JobOrderRequestModel.TechnicianList;
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
                    bool isEditMode = !string.IsNullOrEmpty(JobOrderId);

                    ReloadJobOrderRequestModel();

                    if (!isEditMode) // create mode
                    {
                        JobOrderRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        JobOrderRequestModel.CreatedDateTime = DateTime.Now;
                        JobOrderRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        JobOrderRequestModel.UpdatedDateTime = DateTime.Now;

                        // call create endpoint here...
                        var created = await JobOrderService.CreateAsync(JobOrderRequestModel);

                        // Save services
                        foreach (var s in JobOrderRequestModel.ServiceList)
                        {
                            s.JobOrderId = created.Id;
                            s.ServiceId = s.Service.Id;
                            s.IsPackage = s.IsPackage;
                            s.IsRequired = true; //all items are required once converted.
                            s.JobOrderId = s.JobOrderId;
                            s.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            s.CreatedDateTime = DateTime.Now;
                            s.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            s.UpdatedDateTime = DateTime.Now;

                            await JobOrderServiceService.CreateAsync(s);
                        }

                        if (JobOrderRequestModel.ProductList != null && JobOrderRequestModel.ProductList.Any())
                        {
                            // Save products
                            foreach (var p in JobOrderRequestModel.ProductList)
                            {
                                p.JobOrderId = created.Id;
                                p.ProductId = p.Product.Id;
                                p.JobOrderId = p.JobOrderId;
                                p.IsPackage = p.IsPackage;
                                p.IncentiveSA = p.Product.IncentiveSA;
                                p.IncentiveTech = p.Product.IncentiveTech;
                                p.IsRequired = true; //all items are required once converted.
                                p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                                p.CreatedDateTime = DateTime.Now;
                                p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                                p.UpdatedDateTime = DateTime.Now;

                                await JobOrderProductService.CreateAsync(p);
                            }
                        }

                        // Save package
                        foreach (var p in JobOrderRequestModel.PackageList)
                        {
                            p.PackageId = p.Package.Id;
                            p.JobOrderId = created.Id;
                            p.IncentiveSA = p.IncentiveSA;
                            p.IncentiveTech = p.IncentiveTech;

                            p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.CreatedDateTime = DateTime.Now;
                            p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.UpdatedDateTime = DateTime.Now;

                            await JobOrderPackageService.CreateAsync(p);
                        }

                        // Save technicians
                        foreach (var t in JobOrderRequestModel.TechnicianList)
                        {
                            t.JobOrderId = created.Id;
                            t.TechnicianUserId = t.TechnicianUser.Id;
                            t.JobOrderId = t.JobOrderId;

                            t.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            t.CreatedDateTime = DateTime.Now;
                            t.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            t.UpdatedDateTime = DateTime.Now;

                            await JobOrderTechnicianService.CreateAsync(t);
                        }

                        JobOrderId = created.Id.ToString();
                        await ReloadJobOrderData();

                        IsLoading = false;

                        SnackbarService.Add("Job Order Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/job-orders");

                    }
                    else // update mode
                    {
                        await UpdateJobOrder();

                        await ReloadJobOrderData();

                        SnackbarService.Add("Job Order Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/job-orders");
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

        private async Task UpdateJobOrder()
        {
            int jobOrderId = int.Parse(JobOrderId);

            JobOrderRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
            JobOrderRequestModel.UpdatedDateTime = DateTime.Now;

            // call update endpoint here...
            await JobOrderService.UpdateAsync(JobOrderRequestModel);

            // Detele all current services? Update also include insert inside
            var jobOrderServiceList = await JobOrderServiceService.GetAllJobOrderServiceByJobOrderIdAsync(jobOrderId);
            var jobOrderProductList = await JobOrderProductService.GetAllJobOrderProductByJobOrderIdAsync(jobOrderId);
            var jobOrderPackageList = await JobOrderPackageService.GetAllJobOrderPackageByJobOrderIdAsync(jobOrderId);
            var jobOrderTechnicianList = await JobOrderTechnicianService.GetAllJobOrderTechnicianByJobOrderIdAsync(jobOrderId);

            if (jobOrderServiceList != null && jobOrderServiceList.Any())
            {
                foreach (var del in jobOrderServiceList)
                {
                    //if (!estimateSericeListForSkipDelete.Where(x => x.Id == del.Id).Any())
                    await JobOrderServiceService.DeleteAsync(del.Id);
                }
            }

            if (jobOrderProductList != null && jobOrderProductList.Any())
            {
                foreach (var del in jobOrderProductList)
                {
                    //if (!estiamteProductListForSkipDelete.Where(x => x.Id == del.Id).Any())
                    await JobOrderProductService.DeleteAsync(del.Id);
                }
            }

            if (jobOrderPackageList != null && jobOrderPackageList.Any())
            {
                foreach (var del in jobOrderPackageList)
                {
                    //if (!estiamteProductListForSkipDelete.Where(x => x.Id == del.Id).Any())
                    await JobOrderPackageService.DeleteAsync(del.Id);
                }
            }

            if (jobOrderTechnicianList != null && jobOrderTechnicianList.Any())
            {
                foreach (var del in jobOrderTechnicianList)
                {
                    //if (!estiamteTechnicianListForSkipDelete.Where(x => x.Id == del.Id).Any())
                    await JobOrderTechnicianService.DeleteAsync(del.Id);
                }
            }

            // Save services
            foreach (var s in JobOrderRequestModel.ServiceList)
            {
                s.Id = 0;
                s.ServiceId = s.Service.Id;
                s.JobOrderId = jobOrderId;
                s.IsPackage = s.IsPackage;
                s.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                s.CreatedDateTime = DateTime.Now;
                s.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                s.UpdatedDateTime = DateTime.Now;

                await JobOrderServiceService.CreateAsync(s);
            }

            if (JobOrderRequestModel.ProductList != null && JobOrderRequestModel.ProductList.Any())
            {
                // Detele all current products? Update also include insert inside
                // Save products -> need to check for logic if the product is deleted.
                foreach (var p in JobOrderRequestModel.ProductList)
                {
                    p.Id = 0;
                    p.ProductId = p.Product.Id;
                    p.JobOrderId = jobOrderId;
                    p.IsPackage = p.IsPackage;
                    p.IncentiveSA = p.Product.IncentiveSA;
                    p.IncentiveTech = p.Product.IncentiveTech;

                    p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                    p.CreatedDateTime = DateTime.Now;
                    p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                    p.UpdatedDateTime = DateTime.Now;
                    await JobOrderProductService.CreateAsync(p);
                }
            }

            // Save package
            foreach (var p in JobOrderRequestModel.PackageList)
            {
                p.PackageId = p.Package.Id;
                p.JobOrderId = jobOrderId;
                p.IncentiveSA = p.Package.IncentiveSA;
                p.IncentiveTech = p.Package.IncentiveTech;

                p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                p.CreatedDateTime = DateTime.Now;
                p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                p.UpdatedDateTime = DateTime.Now;

                await JobOrderPackageService.CreateAsync(p);
            }

            // Detele all current Technicians? Update also include insert inside
            // Save Technicians -> need to check for logic if the Technician is deleted.
            foreach (var t in JobOrderRequestModel.TechnicianList)
            {
                t.Id = 0;
                t.TechnicianUserId = t.TechnicianUser.Id;
                t.JobOrderId = jobOrderId;
                t.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                t.CreatedDateTime = DateTime.Now;
                t.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                t.UpdatedDateTime = DateTime.Now;
                await JobOrderTechnicianService.CreateAsync(t);
            }
        }

        private void ReloadJobOrderRequestModel()
        {
            JobOrderRequestModel.CustomerId = JobOrderRequestModel.Customer.Id;
            JobOrderRequestModel.VehicleId = JobOrderRequestModel.Vehicle.Id;
            JobOrderRequestModel.AdvisorUserId = JobOrderRequestModel.AdvisorUser.Id;
            JobOrderRequestModel.EstimatorUserId = JobOrderRequestModel.EstimatorUser.Id;
            JobOrderRequestModel.ApproverUserId = JobOrderRequestModel.ApproverUser.Id;
            JobOrderRequestModel.ServiceGroupId = JobOrderRequestModel.ServiceGroup.Id;
            JobOrderRequestModel.JobStatusId = JobOrderRequestModel.JobStatus.Id;

            JobOrderRequestModel.ProductList = JobOrderProducts;
            JobOrderRequestModel.ServiceList = JobOrderServices;
            JobOrderRequestModel.PackageList = JobOrderPackages;
            JobOrderRequestModel.TechnicianList = JobOrderTechnicians;
        }

        private async Task OnReOpenClick()
        {
            if (string.IsNullOrEmpty(JobOrderId))
                return;

            bool? result = await mbox.ShowAsync();
            var proceed = result == null ? false : true;

            if (proceed)
            {
                IsLoading = true;

                var jobStatusOpen = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                JobOrderRequestModel.JobStatus = jobStatusOpen;
                JobOrderRequestModel.JobStatusId = jobStatusOpen.Id;

                await JobOrderService.UpdateAsync(JobOrderRequestModel);
                SnackbarService.Add("Job Order Successfuly re-OPENED!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                IsLoading = false;
                StateHasChanged();

                NavigationManager.NavigateToCustom("/operations/job-orders", true);
            }
        }

        private async Task OnCancelClick()
        {
            NavigationManager.NavigateToCustom("/operations/job-orders");
        }

        private async Task OnCancelJobOrderClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the this transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                var jobStatusCancelled = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Cancelled)).FirstOrDefault();
                JobOrderRequestModel.JobStatus = jobStatusCancelled;
                JobOrderRequestModel.JobStatusId = jobStatusCancelled.Id;

                await JobOrderService.UpdateAsync(JobOrderRequestModel);

                SnackbarService.Add("Job Order Successfuly Cancelled!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                NavigationManager.NavigateToCustom("/operations/job-orders", true);
            }
        }

        private async Task OnPackageRemoveClick(MudChip<int> chip)
        {
            var removePackage = JobOrderPackages.Where(x => x.Package.Id == chip.Value);
            if (removePackage != null && removePackage.Any())
            {
                JobOrderPackages.Remove(removePackage.FirstOrDefault());

                // remove items
                JobOrderRequestModel.ServiceList.RemoveAll(x => x.PackageId == chip.Value);
                JobOrderRequestModel.ProductList.RemoveAll(x => x.PackageId == chip.Value);

                JobOrderServices = new List<JobOrderServiceDTO>();
                JobOrderServices = JobOrderRequestModel.ServiceList;

                JobOrderProducts = new List<JobOrderProductDTO>();
                JobOrderProducts = JobOrderRequestModel.ProductList;

                StateHasChanged();
            }
        }

        private async Task OnPackageJobOrderClick()
        {
            var package = new PackageDTO();
            var parameters = new DialogParameters<PackageItemComponent> { { x => x.SelectedPackage, package } };
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };

            var dialog = await DialogService.ShowAsync<PackageItemComponent>("test", parameters, options);
            var result = await dialog.Result;

            if (result.Data != null)
            {
                // do the apply package here...
                IsPackage = true;
                var packageData = result.Data.Map<PackageDTO>();

                List<JobOrderPackageDTO> packageToAdd = new List<JobOrderPackageDTO>()
                {
                    new JobOrderPackageDTO()
                    {
                        Package = packageData,
                        JobOrder = new JobOrderDTO()
                    }
                };

                JobOrderPackages.AddRange(packageToAdd);

                // initialize list before looping
                JobOrderRequestModel.ServiceList = JobOrderRequestModel.ServiceList == null
                    ? new List<JobOrderServiceDTO>()
                    : JobOrderRequestModel.ServiceList;

                JobOrderRequestModel.ProductList = JobOrderRequestModel.ProductList == null
                    ? new List<JobOrderProductDTO>()
                    : JobOrderRequestModel.ProductList;

                // loop packages here
                foreach (var pl in packageToAdd)
                {
                    var packageInfo = await PackageService.GetPackageByIdAsync(pl.Package.Id);
                    var packageProductInfo = await PackageProductService.GetAllPackageProductByPackageIdAsync(packageInfo.Id);
                    var packageServiceInfo = await PackageServiceService.GetAllPackageServiceByPackageIdAsync(packageInfo.Id);

                    // Add services from package
                    foreach (var ps in packageServiceInfo)
                    {
                        JobOrderRequestModel.ServiceList.Add(new JobOrderServiceDTO()
                        {
                            Id = ps.Id,
                            PackageId = packageInfo.Id,
                            IsPackage = true,
                            Amount = ps.Amount,
                            Hours = ps.Hours,
                            Rate = ps.Rate,
                            Service = ps.Service
                        });
                    }

                    // Add services from package
                    foreach (var ps in packageProductInfo)
                    {
                        JobOrderRequestModel.ProductList.Add(new JobOrderProductDTO()
                        {
                            Id = ps.Id,
                            PackageId = packageInfo.Id,
                            IsPackage = true,
                            Amount = ps.Amount,
                            IncentiveSA = ps.Product.IncentiveSA,
                            IncentiveTech = ps.Product.IncentiveTech,
                            Qty = ps.Qty,
                            Price = ps.Price,
                            Product = ps.Product
                        });
                    }
                }

                JobOrderServices = new List<JobOrderServiceDTO>();
                JobOrderServices = JobOrderRequestModel.ServiceList;

                JobOrderProducts = new List<JobOrderProductDTO>();
                JobOrderProducts = JobOrderRequestModel.ProductList;

                JobOrderRequestModel.IsPackage = IsPackage;

                // update sub total based on the selected package
                var productListTotalAmount = JobOrderProducts.Sum(x => x.Amount);
                var serviceListTotalAmount = JobOrderServices.Sum(x => x.Amount);

                JobOrderRequestModel.SubTotal = productListTotalAmount + serviceListTotalAmount;

                StateHasChanged();
            }
        }

        private async Task OnJobOrderCompletedClick()
        {
            // validate service, products and technicians
            var isValid = await ValidateSubComponents();

            if (!isValid)
                return;

            mBoxCustomMessage = "Are you sure you want to move this job order to invoice?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {

                ReloadJobOrderRequestModel();
                await UpdateJobOrder();

                var jobStatus = await JobStatusService.GetAllAsync();

                try
                {
                    IMapper mapper = InitializeMapper();
                    var newJobStatus = jobStatus.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                    var newRefNo = await ReferenceNumberHelper.GetRNInvoice(InvoiceService);

                    #region Create New Job Order from Estimate
                    var dto = new InvoiceDTO()
                    {
                        IsPackage = JobOrderRequestModel.IsPackage,
                        InvoiceNo = newRefNo,
                        InvoiceDate = JobOrderRequestModel.TransactionDate,
                        DueDate = JobOrderRequestModel.TransactionDate,
                        JobOrderId = JobOrderRequestModel.Id,
                        JobStatusId = newJobStatus.Id,
                        CustomerId = JobOrderRequestModel.CustomerId,
                        CustomerPO = JobOrderRequestModel.CustomerPO,
                        AdvisorUserId = JobOrderRequestModel.AdvisorUserId,
                        Summary = JobOrderRequestModel.Summary,
                        SubTotal = JobOrderRequestModel.SubTotal,
                        VAT12 = JobOrderRequestModel.VAT12,
                        LaborDiscount = JobOrderRequestModel.LaborDiscount,
                        ProductDiscount = JobOrderRequestModel.ProductDiscount,
                        AdditionalDiscount = JobOrderRequestModel.AdditionalDiscount,
                        TotalAmount = JobOrderRequestModel.TotalAmount,
                        CreatedById = TokenHelper.GetCurrentUserId(await AuthState),
                        CreatedDateTime = DateTime.Now,
                        UpdatedById = TokenHelper.GetCurrentUserId(await AuthState),
                        UpdatedDateTime = DateTime.Now,
                    };

                    var created = await InvoiceService.CreateAsync(dto);

                    // Save package
                    foreach (var s in JobOrderRequestModel.PackageList)
                    {
                        var newDTO = new InvoicePackageDTO()
                        {
                            InvoiceId = created.Id,
                            PackageId = s.Package.Id,
                            IncentiveSA = s.Package.IncentiveSA,
                            IncentiveTech = s.Package.IncentiveTech,
                            CreatedById = TokenHelper.GetCurrentUserId(await AuthState),
                            CreatedDateTime = DateTime.Now,
                            UpdatedById = TokenHelper.GetCurrentUserId(await AuthState),
                            UpdatedDateTime = DateTime.Now
                        };

                        await InvoicePackageService.CreateAsync(newDTO);
                    }
                    #endregion

                    // Update the status of this estimate to converted
                    var convertedStatus = jobStatus.Where(x => x.Name.Equals(Constants.JobStatus.Completed)).FirstOrDefault();
                    JobOrderRequestModel.JobStatus = convertedStatus;
                    JobOrderRequestModel.JobStatusId = convertedStatus.Id;

                    await JobOrderService.UpdateAsync(JobOrderRequestModel);

                    // Update the JobOrderId
                    // Lock the page for converted status

                    NavigationManager.NavigateTo($"/operations/invoices/{created.Id}");

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

        private void OnAddNewItemClick(JobOrderDialogType dialogType)
        {
            bool isEditMode = !string.IsNullOrEmpty(JobOrderId);
            var returnUrl = isEditMode
                ? $"/operations/job-orders/{JobOrderId}"
                : "/operations/job-orders/add";

            if (dialogType == JobOrderDialogType.Customer)
                NavigationManager.NavigateToCustom($"/customers/add?returnUrl={returnUrl}");
            else if (dialogType == JobOrderDialogType.Vehicle)
                NavigationManager.NavigateToCustom($"/vehicles/add?returnUrl={returnUrl}");
            else if (dialogType == JobOrderDialogType.ServiceAdvisor)
                NavigationManager.NavigateToCustom($"/administrators/users/add?returnUrl={returnUrl}");
            else if (dialogType == JobOrderDialogType.ServiceGroup)
                NavigationManager.NavigateToCustom($"/configurations/service-groups/add?returnUrl={returnUrl}");
            else if (dialogType == JobOrderDialogType.JobStatus)
                NavigationManager.NavigateToCustom($"/configurations/job-statuses/add?returnUrl={returnUrl}");
        }

        private void ServiceItemHasChanged(List<JobOrderServiceDTO> e)
        {
            JobOrderServices = e;
            var productTotal = JobOrderProducts == null
                ? 0
                : JobOrderProducts.Sum(x => x.Amount);

            var productAndServiceTotal = e.Sum(x => x.Amount) + productTotal;
            JobOrderRequestModel.SubTotal = (decimal)productAndServiceTotal;

            StateHasChanged();
        }

        private void ProductItemHasChanged(List<JobOrderProductDTO> e)
        {
            JobOrderProducts = e;
            var serviceTotal = JobOrderServices == null
                ? 0
                : JobOrderServices.Sum(x => x.Amount);

            var productAndServiceTotal = e.Sum(x => x.Amount) + serviceTotal;
            JobOrderRequestModel.SubTotal = productAndServiceTotal;

            StateHasChanged();
        }

        private void OnSubTotalChanged(JobOrderDTO dto, string i)
        {
            JobOrderRequestModel.VAT12 = decimal.Parse(i) * 12 / 100;

            JobOrderRequestModel.TotalAmount = JobOrderRequestModel.SubTotal - GetTotalDeductibles();
            StateHasChanged();
        }

        private void OnLaborDiscountChanged(JobOrderDTO dto, decimal i)
        {
            var serviceTotal = JobOrderServices == null
               ? 0
               : JobOrderServices.Sum(x => x.Amount);

            JobOrderRequestModel.LaborDiscount = i;

            JobOrderRequestModel.TotalAmount = JobOrderRequestModel.SubTotal - GetTotalDeductibles();
            StateHasChanged();
        }

        private void OnProductDiscountChanged(JobOrderDTO dto, decimal i)
        {
            var productTotal = JobOrderProducts == null
                ? 0
                : JobOrderProducts.Sum(x => x.Amount);

            JobOrderRequestModel.ProductDiscount = i;

            JobOrderRequestModel.TotalAmount = JobOrderRequestModel.SubTotal - GetTotalDeductibles();
            StateHasChanged();
        }

        private void OnAdditionalDiscountChanged(JobOrderDTO dto, decimal i)
        {
            JobOrderRequestModel.AdditionalDiscount = i;

            JobOrderRequestModel.TotalAmount = JobOrderRequestModel.SubTotal - GetTotalDeductibles();
            StateHasChanged();
        }

        private async Task OnCustomerChanged(JobOrderDTO dto, CustomerDTO i)
        {
            JobOrderRequestModel.Customer = i;
            var vehicleByCustomer = await VehicleService.GetAllVehicleByCustomerIdAsync(i.Id);

            VehicleList = vehicleByCustomer;

            StateHasChanged();
        }

        private decimal GetTotalDeductibles()
        {
            return JobOrderRequestModel.LaborDiscount
                + JobOrderRequestModel.ProductDiscount
                + JobOrderRequestModel.AdditionalDiscount;
        }

        private async Task<bool> ValidateSubComponents()
        {
            if (JobOrderProducts.Any() && JobOrderProducts.Where(x => x.Product.Id.Equals(0)).Any())
            {
                mBoxCustomMessage = "No selected product or product name is empty!";
                await mboxError.ShowAsync();

                return false;
            }

            if (!JobOrderServices.Any())
            {
                // TODO: Ask if service is required in JobOrder
                mBoxCustomMessage = "Service is required!";
                await mboxError.ShowAsync();

                return false;
            }

            if (JobOrderServices.Where(x => x.Service.Id.Equals(0)).Any())
            {
                mBoxCustomMessage = "No selected service or service name is empty!";
                await mboxError.ShowAsync();

                return false;
            }

            if (!JobOrderTechnicians.Any())
            {
                mBoxCustomMessage = "Technician is required!";
                await mboxError.ShowAsync();

                return false;
            }

            if (JobOrderTechnicians.Count() < 1)
            {
                mBoxCustomMessage = "The minimum count of technician is one (1)!";
                await mboxError.ShowAsync();

                return false;
            }

            return true;
        }

        #region Search MudAutoComplete
        private async Task<IEnumerable<CustomerDTO>> SearchCustomer(string filter, CancellationToken token)
        {
            CustomerList = CustomerList.OrderByDescending(x => x.CreatedDateTime).ToList();

            if (string.IsNullOrEmpty(filter))
                return CustomerList;

            return CustomerList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<VehicleDTO>> SearchVehicle(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return VehicleList;

            return VehicleList.Where(i => i.VehicleModel.Description.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<UserDTO>> SearchServiceAdvisor(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ServiceAdvisorList;

            return ServiceAdvisorList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<UserDTO>> SearchEstimator(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return EstimatorList;

            return EstimatorList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<UserDTO>> SearchApprover(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ApproverList;

            return ApproverList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ServiceGroupDTO>> SearchServiceGroup(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ServiceGroupList;

            return ServiceGroupList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
        private async Task<IEnumerable<JobStatusDTO>> SearchJobStatus(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return JobStatusList;

            return JobStatusList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
        #endregion

        private async Task OnEditJobOrderClick()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            var dialog = await DialogService.ShowAsync<OverrideTransactionComponent>("Override Job Order", options);
            var result = await dialog.Result;

            if (result.Data != null)
            {
                var isSupervisorValidated = bool.Parse(result.Data.ToString());

                if (isSupervisorValidated)
                {
                    isJobOrderLocked = false;
                    isJobOrderCompleted = false;
                    form.Disabled = false;

                    // Include currently logged in id in the RequestModel.OverrideByUser (supervisor and up only)
                }
                else
                {
                    mBoxCustomMessage = "Invalid supervisor credential!";
                    await mboxError.ShowAsync();
                }

                StateHasChanged();
            }
        }

        private async Task OnGeneratePdfClick()
        {
            IsLoading = true;

            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            JobOrderReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            JobOrderReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();

            var model = JobOrderRequestModel;
            var technicians = await JobOrderTechnicianService.GetAllJobOrderTechnicianByJobOrderIdAsync(JobOrderRequestModel.Id);
            model.TechnicianList = technicians.Where(x => x.TechnicianUser.Role.Name == "SENIOR TECHNICIAN").ToList();

            await JobOrderReportGenerator.Generate(model, JSRuntime, companyData);

            IsLoading = false;
        }

        #region Mapper
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JobOrder, JobOrderDTO>();
                cfg.CreateMap<Entities.JobStatus, JobStatusDTO>();
                cfg.CreateMap<Customer, CustomerDTO>();
                cfg.CreateMap<Vehicle, VehicleDTO>();
                cfg.CreateMap<VehicleModel, VehicleModelDTO>();
                cfg.CreateMap<VehicleMake, VehicleMakeDTO>();
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<ServiceGroup, ServiceGroupDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<Role, RoleDTO>();

                cfg.CreateMap<RoleDTO, Role>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<JobOrderDTO, JobOrder>();
                cfg.CreateMap<JobStatusDTO, Entities.JobStatus>();
                cfg.CreateMap<CustomerDTO, Customer>();
                cfg.CreateMap<VehicleDTO, Vehicle>();
                cfg.CreateMap<VehicleModelDTO, VehicleModel>();
                cfg.CreateMap<VehicleMakeDTO, VehicleMake>();
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<ServiceGroupDTO, ServiceGroup>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }
        #endregion
    }
}