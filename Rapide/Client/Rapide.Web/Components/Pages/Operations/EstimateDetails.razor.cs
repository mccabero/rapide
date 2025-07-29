using System.Collections.Generic;
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
    public partial class EstimateDetails
    {
        #region Parameters
        [Parameter]
        public string? EstimateId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private IEstimateService EstimateService { get; set; }
        [Inject]
        private IEstimatePackageService EstimatePackageService { get; set; }
        [Inject]
        private IEstimateServiceService EstimateServiceService { get; set; }
        [Inject]
        private IEstimateProductService EstimateProductService { get; set; }
        [Inject]
        private IEstimateTechnicianService EstimateTechnicianService { get; set; }
        [Inject]
        private IJobStatusService JobStatusService { get; set; }
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
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IDialogService DialogService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        #endregion

        #region Private Properties
        private EstimateDTO EstimateRequestModel { get; set; } = new();

        private List<JobStatusDTO> JobStatusList { get; set; } = new();
        private List<CustomerDTO> CustomerList { get; set; } = new();
        private List<VehicleDTO> VehicleList { get; set; } = new();
        private List<UserDTO> ServiceAdvisorList { get; set; } = new();
        private List<UserDTO> EstimatorList { get; set; } = new();
        private List<UserDTO> ApproverList { get; set; } = new();
        private List<ServiceGroupDTO> ServiceGroupList { get; set; } = new();
       

        private MudMessageBox mboxCustom { get; set; }
        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }
        private bool IsEditMode { get; set; }

        // From child components
        public List<EstimateServiceDTO> EstimateServices { get; set; } = new();
        public List<EstimateProductDTO> EstimateProducts { get; set; } = new();
        public List<EstimateTechnicianDTO> EstimateTechnicians { get; set; } = new();
        private List<EstimatePackageDTO> EstimatePackages { get; set; } = new();

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private string JobStatusName = string.Empty;
        private string CustomerName = string.Empty;
        private bool isEstimateLocked = false;

        private bool IsPackage = false;
        private bool isBigThreeRoles = false;
        private bool isOIC = false;
        private bool isCashier = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var userList = await UserService.GetAllUserRoleAsync();
            var userWithRoles = new List<UserDTO>();

            foreach (var u in userList)
            {
                var userInfo = new UserDTO();

                userInfo = u;
                userInfo.UserRoles = await UserRolesService.GetUserRolesByUserIdAsync(u.Id);
            }

            IsEditMode = !string.IsNullOrEmpty(EstimateId);
            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);
            isOIC = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.OIC);
            isCashier = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier);

            CustomerList = await CustomerService.GetAllAsync();
            ServiceAdvisorList = userList.Where(x => x.IsActive && x.UserRoles.Any(x => x.Role.Name.Equals(Constants.UserRoles.ServiceAdvisor))).ToList();
            EstimatorList = userList.Where(x => x.IsActive && x.UserRoles.Any(x => x.Role.Name.Equals(Constants.UserRoles.Estimator))).ToList();
            ApproverList = userList.Where(x => x.IsActive && x.UserRoles.Any(x => x.Role.Name.Equals(Constants.UserRoles.Supervisor))).ToList();
            ServiceGroupList = await ServiceGroupService.GetAllAsync();
            JobStatusList = await JobStatusService.GetAllAsync();

            if (IsEditMode)
            {
                await ReloadEstimateData();
                CustomerName = $"{EstimateRequestModel.Customer.FirstName} {EstimateRequestModel.Customer.LastName}";

                // creteria of locked based on status.
                isEstimateLocked = 
                    EstimateRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Converted) ||
                    EstimateRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Completed) ||
                    EstimateRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Cancelled) ||
                    isViewOnly;
                
                form.Disabled = isEstimateLocked;
                IsEditMode = !isEstimateLocked;
            }
            else
            {
                var currentUserId = TokenHelper.GetCurrentUserId(await AuthState);
                CustomerName = "New Estimate";
                EstimateRequestModel.JobStatus = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                EstimateRequestModel.TransactionDate = DateTime.Now;
                EstimateRequestModel.ExpirationDate = DateTime.Now.AddMonths(1);
                JobStatusName = EstimateRequestModel.JobStatus.Name;

                EstimateRequestModel.EstimatorUser = userList.Where(x => x.Id == currentUserId).FirstOrDefault();
                EstimateRequestModel.AdvisorUser = userList.Where(x => x.Id == currentUserId).FirstOrDefault();

                EstimateRequestModel.ReferenceNo = await ReferenceNumberHelper.GetRNEstimate(EstimateService);

            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task ReloadEstimateData()
        {
            // Get data by id
            EstimateRequestModel = await EstimateService.GetEstimateByIdAsync(int.Parse(EstimateId));
            EstimateRequestModel.CustomerPO = EstimateRequestModel.CustomerPO == null
                ? string.Empty
                : EstimateRequestModel.CustomerPO;

            JobStatusName = EstimateRequestModel.JobStatus.Name;

            VehicleList = await VehicleService.GetAllVehicleByCustomerIdAsync(EstimateRequestModel.CustomerId);

            // Package data
            IsPackage = EstimateRequestModel.IsPackage;
            if (IsPackage)
            {
                //var packageInfo = await PackageService.GetPackageByIdAsync((int)EstimateRequestModel.PackageId);
                //PackageName = packageInfo.Name;

                var packageListInfo = await EstimatePackageService.GetAllEstimatePackageByEstimateIdAsync(EstimateRequestModel.Id);
                EstimatePackages = packageListInfo;
            }

            EstimateRequestModel.ProductList = await EstimateProductService.GetAllEstimateProductByEstimateIdAsync(EstimateRequestModel.Id);
            EstimateRequestModel.ServiceList = await EstimateServiceService.GetAllEstimateServiceByEstimateIdAsync(EstimateRequestModel.Id);
            EstimateRequestModel.TechnicianList = await EstimateTechnicianService.GetAllEstimateTechnicianByEstimateIdAsync(EstimateRequestModel.Id);
            EstimateRequestModel.PackageList = EstimatePackages;

            if (EstimatePackages == null)
            {
                EstimateRequestModel.IsPackage = false;
                IsPackage = false;
            }

            EstimateServices = EstimateRequestModel.ServiceList == null
                ? new List<EstimateServiceDTO>()
                : EstimateRequestModel.ServiceList;

            EstimateProducts = EstimateRequestModel.ProductList == null
                ? new List<EstimateProductDTO>()
                : EstimateRequestModel.ProductList;

            EstimateTechnicians = EstimateRequestModel.TechnicianList == null
                ? new List<EstimateTechnicianDTO>()
                : EstimateRequestModel.TechnicianList;
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
                    bool isEditMode = !string.IsNullOrEmpty(EstimateId);

                    ReloadEstimateRequestModel();

                    if (!isEditMode) // create mode
                    {
                        EstimateRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        EstimateRequestModel.CreatedDateTime = DateTime.Now;
                        EstimateRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        EstimateRequestModel.UpdatedDateTime = DateTime.Now;

                        // call create endpoint here...
                        var created = await EstimateService.CreateAsync(EstimateRequestModel);

                        // Save services
                        foreach (var s in EstimateRequestModel.ServiceList)
                        {
                            s.IsPackage = s.IsPackage;
                            s.IsRequired = s.IsRequired;
                            s.EstimateId = created.Id;
                            s.ServiceId = s.Service.Id;
                            s.EstimateId = s.EstimateId;
                            s.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            s.CreatedDateTime = DateTime.Now;
                            s.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            s.UpdatedDateTime = DateTime.Now;

                            await EstimateServiceService.CreateAsync(s);
                        }

                        // Save products
                        if (EstimateRequestModel.ProductList != null & EstimateRequestModel.ProductList.Any())
                        {
                            foreach (var p in EstimateRequestModel.ProductList)
                            {
                                p.IsPackage = p.IsPackage;
                                p.IsRequired = p.IsRequired;
                                p.EstimateId = created.Id;
                                p.ProductId = p.Product.Id;
                                p.EstimateId = p.EstimateId;
                                p.IncentiveSA = p.IncentiveSA;
                                p.IncentiveTech = p.IncentiveTech;

                                p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                                p.CreatedDateTime = DateTime.Now;
                                p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                                p.UpdatedDateTime = DateTime.Now;

                                await EstimateProductService.CreateAsync(p);
                            }
                        }

                        // Save packages
                        foreach (var p in EstimateRequestModel.PackageList)
                        {
                            p.PackageId = p.Package.Id;
                            p.EstimateId = created.Id;
                            p.IncentiveSA = p.IncentiveSA;
                            p.IncentiveTech = p.IncentiveTech;

                            p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.CreatedDateTime = DateTime.Now;
                            p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            p.UpdatedDateTime = DateTime.Now;

                            await EstimatePackageService.CreateAsync(p);
                        }

                        // Save technicians
                        foreach (var t in EstimateRequestModel.TechnicianList)
                        {
                            t.EstimateId = created.Id;
                            t.TechnicianUserId = t.TechnicianUser.Id;
                            t.EstimateId = t.EstimateId;

                            t.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            t.CreatedDateTime = DateTime.Now;
                            t.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            t.UpdatedDateTime = DateTime.Now;

                            await EstimateTechnicianService.CreateAsync(t);
                        }

                        EstimateId = created.Id.ToString();
                        await ReloadEstimateData();

                        IsLoading = false;

                        SnackbarService.Add("Estimate Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/estimates", true);

                    }
                    else // update mode
                    {
                        await UpdateEstimate();
                        await ReloadEstimateData();

                        SnackbarService.Add("Estimate Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/estimates", true);
                        IsLoading = false;

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

        private void ReloadEstimateRequestModel()
        {
            EstimateRequestModel.CustomerId = EstimateRequestModel.Customer.Id;
            EstimateRequestModel.VehicleId = EstimateRequestModel.Vehicle.Id;
            EstimateRequestModel.AdvisorUserId = EstimateRequestModel.AdvisorUser.Id;
            EstimateRequestModel.EstimatorUserId = EstimateRequestModel.EstimatorUser.Id;
            EstimateRequestModel.ApproverUserId = EstimateRequestModel.ApproverUser.Id;
            EstimateRequestModel.ServiceGroupId = EstimateRequestModel.ServiceGroup.Id;
            EstimateRequestModel.JobStatusId = EstimateRequestModel.JobStatus.Id;

            EstimateRequestModel.ProductList = EstimateProducts;
            EstimateRequestModel.ServiceList = EstimateServices;
            EstimateRequestModel.TechnicianList = EstimateTechnicians;
            EstimateRequestModel.PackageList = EstimatePackages;
        }

        private async Task UpdateEstimate()
        {
            ReloadEstimateRequestModel();

            int estimateId = int.Parse(EstimateId);

            if (EstimateRequestModel.IsPackage)
            {
                // update packagelist incentives
                var newPackageList = new List<EstimatePackageDTO>();
                foreach (var ep in EstimatePackages)
                {
                    ep.IncentiveSA = ep.Package.IncentiveSA;
                    ep.IncentiveTech = ep.Package.IncentiveTech;

                    newPackageList.Add(ep);
                }
                EstimateRequestModel.PackageList = newPackageList;
            }

            // update productList incentives
            var newProductList = new List<EstimateProductDTO>();
            foreach (var ep in EstimateProducts)
            {
                ep.IncentiveSA = ep.Product.IncentiveSA;
                ep.IncentiveTech = ep.Product.IncentiveTech;

                newProductList.Add(ep);
            }

            EstimateRequestModel.ProductList = newProductList;
            EstimateRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
            EstimateRequestModel.UpdatedDateTime = DateTime.Now;

            // call update endpoint here...
            await EstimateService.UpdateAsync(EstimateRequestModel);

            // Detele all current services? Update also include insert inside
            var estimateServiceList = await EstimateServiceService.GetAllEstimateServiceByEstimateIdAsync(estimateId);
            var estimateProductList = await EstimateProductService.GetAllEstimateProductByEstimateIdAsync(estimateId);
            var estimatePackageList = await EstimatePackageService.GetAllEstimatePackageByEstimateIdAsync(estimateId);
            var estimateTechnicianList = await EstimateTechnicianService.GetAllEstimateTechnicianByEstimateIdAsync(estimateId);

            if (estimateServiceList != null && estimateServiceList.Any())
            {
                foreach (var del in estimateServiceList)
                {
                    //if (!estimateSericeListForSkipDelete.Where(x => x.Id == del.Id).Any())
                    await EstimateServiceService.DeleteAsync(del.Id);
                }
            }

            if (estimateProductList != null && estimateProductList.Any())
            {
                foreach (var del in estimateProductList)
                {
                    //if (!estimateProductListForSkipDelete.Where(x => x.Id == del.Id).Any())
                    await EstimateProductService.DeleteAsync(del.Id);
                }
            }

            if (estimatePackageList != null && estimatePackageList.Any())
            {
                foreach (var del in estimatePackageList)
                {
                    //if (!estimatePackageListForSkipDelete.Where(x => x.Id == del.Id).Any())
                    await EstimatePackageService.DeleteAsync(del.Id);
                }
            }

            if (estimateTechnicianList != null && estimateTechnicianList.Any())
            {
                foreach (var del in estimateTechnicianList)
                {
                    //if (!estimateTechnicianListForSkipDelete.Where(x => x.Id == del.Id).Any())
                    await EstimateTechnicianService.DeleteAsync(del.Id);
                }
            }

            // Save services
            foreach (var s in EstimateRequestModel.ServiceList)
            {
                s.IsPackage = s.IsPackage;
                s.IsRequired = s.IsRequired;
                s.ServiceId = s.Service.Id;
                s.EstimateId = estimateId;
                s.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                s.CreatedDateTime = DateTime.Now;
                s.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                s.UpdatedDateTime = DateTime.Now;
                await EstimateServiceService.CreateAsync(s);
            }

            if (EstimateRequestModel.ProductList != null && EstimateRequestModel.ProductList.Any())
            {
                // Detele all current products? Update also include insert inside
                // Save products -> need to check for logic if the product is deleted.
                foreach (var p in EstimateRequestModel.ProductList)
                {
                    p.IsPackage = p.IsPackage;
                    p.IsRequired = p.IsRequired;
                    p.ProductId = p.Product.Id;
                    p.IncentiveSA = p.IncentiveSA;
                    p.IncentiveTech = p.IncentiveTech;
                    p.EstimateId = estimateId;
                    p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                    p.CreatedDateTime = DateTime.Now;
                    p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                    p.UpdatedDateTime = DateTime.Now;
                    await EstimateProductService.CreateAsync(p);
                }
            }

            if (EstimateRequestModel.IsPackage)
            {
                // Save products -> need to check for logic if the product is deleted.
                foreach (var p in EstimateRequestModel.PackageList)
                {
                    p.PackageId = p.Package.Id;
                    p.EstimateId = estimateId;
                    p.IncentiveSA = p.IncentiveSA;
                    p.IncentiveTech = p.IncentiveTech;
                    p.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                    p.CreatedDateTime = DateTime.Now;
                    p.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                    p.UpdatedDateTime = DateTime.Now;
                    await EstimatePackageService.CreateAsync(p);
                }
            }

            // Detele all current Technicians? Update also include insert inside
            // Save Technicians -> need to check for logic if the Technician is deleted.
            foreach (var t in EstimateRequestModel.TechnicianList)
            {
                t.TechnicianUserId = t.TechnicianUser.Id;
                t.EstimateId = estimateId;
                t.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                t.CreatedDateTime = DateTime.Now;
                t.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                t.UpdatedDateTime = DateTime.Now;
                await EstimateTechnicianService.CreateAsync(t);
            }
        }

        private async Task OnReOpenClick()
        {
            if (string.IsNullOrEmpty(EstimateId))
                return;

            bool? result = await mbox.ShowAsync();
            var proceed = result == null ? false : true;

            if (proceed)
            {
                IsLoading = true;

                var jobStatusOpen = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                EstimateRequestModel.JobStatus = jobStatusOpen;
                EstimateRequestModel.JobStatusId = jobStatusOpen.Id;

                await EstimateService.UpdateAsync(EstimateRequestModel);
                SnackbarService.Add("Estimate Successfuly re-OPENED!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                IsLoading = false;
                StateHasChanged();

                NavigationManager.NavigateToCustom("/operations/estimates", true);
            }
        }
        
        private async Task OnCancelClick()
        {
            NavigationManager.NavigateToCustom("/operations/estimates");
        }

        private async Task OnNewEstimateClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the current transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
                NavigationManager.NavigateToCustom("/operations/estimates/add", true);
        }

        private async Task OnCancelEstimateClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the this transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                var jobStatusCancelled = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Cancelled)).FirstOrDefault();
                EstimateRequestModel.JobStatus = jobStatusCancelled;
                EstimateRequestModel.JobStatusId = jobStatusCancelled.Id;

                await EstimateService.UpdateAsync(EstimateRequestModel);

                SnackbarService.Add("Estimate Successfuly Cancelled!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                NavigationManager.NavigateToCustom("/operations/estimates", true);
            }
        }

        private async Task OnPackageEstimateClick()
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

                List<EstimatePackageDTO> packageToAdd = new List<EstimatePackageDTO>() 
                {
                    new EstimatePackageDTO()
                    {
                        IncentiveSA = packageData.IncentiveSA,
                        IncentiveTech = packageData.IncentiveTech,
                        Package = packageData,
                        Estimate = new EstimateDTO()
                    }
                };

                EstimatePackages.AddRange(packageToAdd);

                EstimateRequestModel.NextOdometerReminder = packageData.NextServiceReminderDays;

                // initialize list before looping
                EstimateRequestModel.ServiceList = EstimateRequestModel.ServiceList == null
                    ? new List<EstimateServiceDTO>()
                    : EstimateRequestModel.ServiceList;

                EstimateRequestModel.ProductList = EstimateRequestModel.ProductList == null
                    ? new List<EstimateProductDTO>()
                    : EstimateRequestModel.ProductList;

                EstimateRequestModel.PackageList = EstimatePackages;


                // loop packages here
                foreach (var pl in packageToAdd)
                {
                    var packageInfo = await PackageService.GetPackageByIdAsync(pl.Package.Id);
                    var packageProductInfo = await PackageProductService.GetAllPackageProductByPackageIdAsync(packageInfo.Id);
                    var packageServiceInfo = await PackageServiceService.GetAllPackageServiceByPackageIdAsync(packageInfo.Id);

                    // Add services from package
                    foreach (var ps in packageServiceInfo)
                    {
                        EstimateRequestModel.ServiceList.Add(new EstimateServiceDTO()
                        {
                            Id = ps.Id,
                            PackageId = packageInfo.Id,
                            IsPackage = true,
                            IsRequired = true,
                            Amount = ps.Amount,
                            Hours = ps.Hours,
                            Rate = ps.Rate,
                            Service = ps.Service
                        });
                    }

                    // Add services from package
                    foreach (var ps in packageProductInfo)
                    {
                        EstimateRequestModel.ProductList.Add(new EstimateProductDTO()
                        {
                            Id = ps.Id,
                            PackageId = packageInfo.Id,
                            IsPackage = true,
                            IsRequired = true,
                            Amount = ps.Amount,
                            IncentiveSA = ps.Product.IncentiveSA,
                            IncentiveTech = ps.Product.IncentiveTech,
                            Qty = ps.Qty,
                            Price = ps.Price,
                            Product = ps.Product
                        });
                    }
                }

                EstimateServices = new List<EstimateServiceDTO>();
                EstimateServices = EstimateRequestModel.ServiceList;

                EstimateProducts = new List<EstimateProductDTO>();
                EstimateProducts = EstimateRequestModel.ProductList;

                EstimateRequestModel.IsPackage = IsPackage;

                // update sub total based on the selected package
                var productListTotalAmount = EstimateProducts.Sum(x => x.Amount);
                var serviceListTotalAmount = EstimateServices.Sum(x => x.Amount);

                EstimateRequestModel.SubTotal = productListTotalAmount + serviceListTotalAmount;

                StateHasChanged();
            }
        }

        private async Task OnPackageRemoveClick(MudChip<int> chip)
        {
            var removePackage = EstimatePackages.Where(x => x.Package.Id == chip.Value);
            if (removePackage != null && removePackage.Any())
            {
                EstimatePackages.Remove(removePackage.FirstOrDefault());

                // remove items
                EstimateRequestModel.ServiceList.RemoveAll(x => x.PackageId == chip.Value);
                EstimateRequestModel.ProductList.RemoveAll(x => x.PackageId == chip.Value);

                EstimateServices = new List<EstimateServiceDTO>();
                EstimateServices = EstimateRequestModel.ServiceList;

                EstimateProducts = new List<EstimateProductDTO>();
                EstimateProducts = EstimateRequestModel.ProductList;

                StateHasChanged();
            }
        }

        private async Task OnCustomerApprovedClick()
        {
            var isValidated = await ValidateSubComponents();
            if (!isValidated)
                return;

            mBoxCustomMessage = "Are you sure you want to move this estimate to job order?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                await UpdateEstimate();

                var jobStatus = await JobStatusService.GetAllAsync();

                try
                {
                    IMapper mapper = InitializeMapper();
                    var newJobStatus = jobStatus.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                    var newRefNo = await ReferenceNumberHelper.GetRNJobOrder(JobOrderService);

                    #region Create New Job Order from Estimate
                    var dto = new JobOrderDTO()
                    {
                        IsPackage = EstimateRequestModel.IsPackage,
                        IsPaid = false,
                        EstimateId = EstimateRequestModel.Id,
                        ReferenceNo = newRefNo,
                        TransactionDate = EstimateRequestModel.TransactionDate,
                        ExpirationDate = EstimateRequestModel.ExpirationDate,
                        JobStatus = newJobStatus,
                        JobStatusId = newJobStatus.Id,
                        CustomerId = EstimateRequestModel.CustomerId,
                        VehicleId = EstimateRequestModel.VehicleId,
                        AdvisorUserId = EstimateRequestModel.AdvisorUserId,
                        EstimatorUserId = EstimateRequestModel.EstimatorUserId,
                        ApproverUserId = EstimateRequestModel.ApproverUserId,
                        ServiceGroupId = EstimateRequestModel.ServiceGroupId,
                        Odometer = EstimateRequestModel.Odometer,
                        NextOdometerReminder = EstimateRequestModel.NextOdometerReminder,
                        CustomerPO = EstimateRequestModel.CustomerPO,
                        Summary = EstimateRequestModel.Summary,
                        SubTotal = EstimateRequestModel.SubTotal,
                        VAT12 = EstimateRequestModel.VAT12,
                        LaborDiscount = EstimateRequestModel.LaborDiscount,
                        ProductDiscount = EstimateRequestModel.ProductDiscount,
                        AdditionalDiscount = EstimateRequestModel.AdditionalDiscount,
                        TotalAmount = EstimateRequestModel.TotalAmount,
                        CreatedById = TokenHelper.GetCurrentUserId(await AuthState),
                        CreatedDateTime = DateTime.Now,
                        UpdatedById = TokenHelper.GetCurrentUserId(await AuthState),
                        UpdatedDateTime = DateTime.Now,
                    };

                    var created = await JobOrderService.CreateAsync(dto);

                    // Save services
                    foreach (var s in EstimateRequestModel.ServiceList)
                    {
                        var newDTO = new JobOrderServiceDTO()
                        {
                            JobOrderId = created.Id,
                            ServiceId = s.Service.Id,
                            IsPackage = s.IsPackage,
                            PackageId = s.PackageId,
                            IsRequired = true, //all items are required once converted.
                            Rate = s.Rate,
                            Hours = s.Hours,
                            Amount = s.Amount,
                            CreatedById = TokenHelper.GetCurrentUserId(await AuthState),
                            CreatedDateTime = DateTime.Now,
                            UpdatedById = TokenHelper.GetCurrentUserId(await AuthState),
                            UpdatedDateTime = DateTime.Now
                        };

                        await JobOrderServiceService.CreateAsync(newDTO);
                    }

                    if (EstimateRequestModel.ProductList != null && EstimateRequestModel.ProductList.Any())
                    { 
                        // Save products
                        foreach (var s in EstimateRequestModel.ProductList)
                        {
                            var newDTO = new JobOrderProductDTO()
                            {
                                JobOrderId = created.Id,
                                ProductId = s.Product.Id,
                                IsPackage = s.IsPackage,
                                PackageId = s.PackageId,
                                IsRequired = true, //all items are required once converted.
                                Price = s.Price,
                                Qty = s.Qty,
                                Amount = s.Amount,
                                IncentiveSA = s.Product.IncentiveSA,
                                IncentiveTech = s.Product.IncentiveTech,
                                CreatedById = TokenHelper.GetCurrentUserId(await AuthState),
                                CreatedDateTime = DateTime.Now,
                                UpdatedById = TokenHelper.GetCurrentUserId(await AuthState),
                                UpdatedDateTime = DateTime.Now
                            };

                            await JobOrderProductService.CreateAsync(newDTO);
                        }
                    }

                    // Save package
                    foreach (var s in EstimateRequestModel.PackageList)
                    {
                        var newDTO = new JobOrderPackageDTO()
                        {
                            JobOrderId = created.Id,
                            PackageId = s.Package.Id,
                            IncentiveSA = s.Package.IncentiveSA,
                            IncentiveTech = s.Package.IncentiveTech,
                            CreatedById = TokenHelper.GetCurrentUserId(await AuthState),
                            CreatedDateTime = DateTime.Now,
                            UpdatedById = TokenHelper.GetCurrentUserId(await AuthState),
                            UpdatedDateTime = DateTime.Now
                        };

                        await JobOrderPackageService.CreateAsync(newDTO);
                    }

                    // Save technicians
                    foreach (var s in EstimateRequestModel.TechnicianList)
                    {
                        var newDTO = new JobOrderTechnicianDTO()
                        {
                            JobOrderId = created.Id,
                            TechnicianUserId = s.TechnicianUser.Id,
                            CreatedById = TokenHelper.GetCurrentUserId(await AuthState),
                            CreatedDateTime = DateTime.Now,
                            UpdatedById = TokenHelper.GetCurrentUserId(await AuthState),
                            UpdatedDateTime = DateTime.Now
                        };

                        await JobOrderTechnicianService.CreateAsync(newDTO);
                    }
                    #endregion
                
                    // Update the status of this estimate to converted
                    var convertedStatus = jobStatus.Where(x => x.Name.Equals(Constants.JobStatus.Converted)).FirstOrDefault();
                    EstimateRequestModel.JobStatus = convertedStatus;
                    EstimateRequestModel.JobStatusId = convertedStatus.Id;

                    await EstimateService.UpdateAsync(EstimateRequestModel);

                    // Update the JobOrderId
                    // Lock the page for converted status

                    NavigationManager.NavigateTo($"/operations/job-orders/{created.Id}");

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

        private void OnAddNewItemClick(EstimateDialogType dialogType)
        {
            bool isEditMode = !string.IsNullOrEmpty(EstimateId);
            var returnUrl = isEditMode
                ? $"/operations/estimates/{EstimateId}"
                : "/operations/estimates/add";

            if (dialogType == EstimateDialogType.Customer)
                NavigationManager.NavigateToCustom($"/customers/add?returnUrl={returnUrl}");
            else if (dialogType == EstimateDialogType.Vehicle)
                NavigationManager.NavigateToCustom($"/vehicles/add?returnUrl={returnUrl}");
            else if (dialogType == EstimateDialogType.User)
                NavigationManager.NavigateToCustom($"/administrators/users/add?returnUrl={returnUrl}");
            else if (dialogType == EstimateDialogType.ServiceGroup)
                NavigationManager.NavigateToCustom($"/configurations/service-groups/add?returnUrl={returnUrl}");
            else if (dialogType == EstimateDialogType.JobStatus)
                NavigationManager.NavigateToCustom($"/configurations/job-statuses/add?returnUrl={returnUrl}");
        }

        private void ServiceItemHasChanged(List<EstimateServiceDTO> e)
        {
            EstimateServices = e;
            var productTotal = EstimateProducts == null
                ? 0
                : EstimateProducts.Sum(x => x.Amount);

            var estimatesToAdd = e;

            var productAndServiceTotal = e.Sum(x => x.Amount) + productTotal;
            EstimateRequestModel.SubTotal = productAndServiceTotal;

            if (EstimateRequestModel.ServiceList == null || EstimateRequestModel.ServiceList.Count <= 0)
            {
                EstimateRequestModel.ServiceList = new List<EstimateServiceDTO>();
                EstimateRequestModel.ServiceList.AddRange(e);
            }
            else
            {
                EstimateRequestModel.ServiceList = e;
            }
                

            StateHasChanged();
        }
        
        private void ProductItemHasChanged(List<EstimateProductDTO> e)
        {
            EstimateProducts = e;
            var serviceTotal = EstimateServices == null
                ? 0
                : EstimateServices.Sum(x => x.Amount);

            var productAndServiceTotal = e.Sum(x => x.Amount) + serviceTotal;
            EstimateRequestModel.SubTotal = productAndServiceTotal;

            if (EstimateRequestModel.ProductList == null || EstimateRequestModel.ProductList.Count <= 0)
            {
                EstimateRequestModel.ProductList = new List<EstimateProductDTO>();
                EstimateRequestModel.ProductList.AddRange(e);
            }
            else
            {
                EstimateRequestModel.ProductList = e;
            }

            StateHasChanged();
        }

        private void OnSubTotalChanged(EstimateDTO dto, string i)
        {
            EstimateRequestModel.VAT12 = decimal.Parse(i) * 12 / 100;

            EstimateRequestModel.TotalAmount = EstimateRequestModel.SubTotal - GetTotalDeductibles();
            StateHasChanged();
        }

        private void OnLaborDiscountChanged(EstimateDTO dto, decimal i)
        {
            var serviceTotal = EstimateServices == null
               ? 0
               : EstimateServices.Sum(x => x.Amount);

            EstimateRequestModel.LaborDiscount = i;

            EstimateRequestModel.TotalAmount = EstimateRequestModel.SubTotal - GetTotalDeductibles();
            StateHasChanged();
        }

        private void OnProductDiscountChanged(EstimateDTO dto, decimal i)
        {
            var productTotal = EstimateProducts == null
                ? 0
                : EstimateProducts.Sum(x => x.Amount);

            EstimateRequestModel.ProductDiscount = i;

            EstimateRequestModel.TotalAmount = EstimateRequestModel.SubTotal - GetTotalDeductibles();
            StateHasChanged();
        }

        private void OnAdditionalDiscountChanged(EstimateDTO dto, decimal i)
        {
            EstimateRequestModel.AdditionalDiscount = i;

            EstimateRequestModel.TotalAmount = EstimateRequestModel.SubTotal - GetTotalDeductibles();
            StateHasChanged();
        }

        private async Task OnCustomerChanged(EstimateDTO dto, CustomerDTO i)
        {
            EstimateRequestModel.Customer = i;
            var vehicleByCustomer = await VehicleService.GetAllVehicleByCustomerIdAsync(i.Id);

            VehicleList = vehicleByCustomer;

            StateHasChanged();
        }

        private decimal GetTotalDeductibles()
        { 
            return EstimateRequestModel.LaborDiscount
                + EstimateRequestModel.ProductDiscount
                + EstimateRequestModel.AdditionalDiscount;
        }

        private async Task<bool> ValidateSubComponents()
        {
            if (EstimateProducts.Any() && EstimateProducts.Where(x => x.Product.Id.Equals(0)).Any())
            {
                mBoxCustomMessage = "No selected product or product name is empty!";
                await mboxError.ShowAsync();

                return false;
            }

            if (!EstimateServices.Any())
            {
                // TODO: Ask if service is required in estimate
                mBoxCustomMessage = "Service is required!";
                await mboxError.ShowAsync();

                return false;
            }

            if (EstimateServices.Where(x => x.Service.Id.Equals(0)).Any())
            {
                mBoxCustomMessage = "No selected service or service name is empty!";
                await mboxError.ShowAsync();

                return false;
            }

            if (!EstimateTechnicians.Any())
            {
                mBoxCustomMessage = "Technician is required!";
                await mboxError.ShowAsync();

                return false;
            }

            if (EstimateTechnicians.Count() < 1)
            {
                mBoxCustomMessage = "Please select at lease one (1) technician!";
                await mboxError.ShowAsync();

                return false;
            }

            return true;
        }

        private async Task OnGeneratePdfClick()
        {
            IsLoading = true;

            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            EstimateReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            EstimateReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();

            var model = EstimateRequestModel;
            var technicians = await EstimateTechnicianService.GetAllEstimateTechnicianByEstimateIdAsync(EstimateRequestModel.Id);
            model.TechnicianList = technicians.Where(x => x.TechnicianUser.Role.Name == "SENIOR TECHNICIAN").ToList();

            await EstimateReportGenerator.Generate(model, JSRuntime, companyData);

            IsLoading = false;
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