using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Services;
using Rapide.Web.Components.Pages.SystemConfiguration;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.PdfReportGenerator;
using System.Transactions;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class InspectionDetails
    {
        #region Parameters
        [Parameter]
        public string? InspectionId { get; set; }
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
        private IDialogService DialogService { get; set; }
        [Inject]
        private IWebHostEnvironment _environment { get; set; }

        [Inject]
        private IInspectionTechnicianService InspectionTechnicianService { get; set; }

        [Inject]
        private IInspectionService InspectionService { get; set; }
        [Inject]
        private ICustomerService CustomerService { get; set; }
        [Inject]
        private IVehicleService VehicleService { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IUserRolesService UserRolesService { get; set; }
        [Inject]
        private IJobStatusService JobStatusService { get; set; }
        [Inject]
        private IEstimateService EstimateService { get; set; }
        [Inject]
        private IServiceGroupService ServiceGroupService { get; set; }
        [Inject]
        private IEstimateTechnicianService EstimateTechnicianService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        #endregion

        #region Private Properties
        private InspectionDTO InspectionRequestModel { get; set; }
        private List<InspectionGroupModel> InspectionTemplate { get; set; }

        private List<JobStatusDTO> JobStatusList { get; set; } = new();
        private List<CustomerDTO> CustomerList { get; set; } = new();
        private List<VehicleDTO> VehicleList { get; set; } = new();
        private List<UserDTO> InspectorList { get; set; } = new();

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
        public List<InspectionTechnicianDTO> InspectionTechnicians { get; set; } = new();

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private string JobStatusName = string.Empty;
        private string CustomerName = string.Empty;
        private bool isInspectionLocked = false;

        private IList<IBrowserFile> files = new List<IBrowserFile>();
        private List<string> uploadedFiles = new List<string>();
        private string imageSource = string.Empty;


        private bool arrows = true;
        private bool bullets = true;
        private bool enableSwipeGesture = true;
        private bool autocycle = true;
        private Transition transition = Transition.Slide;

        private IList<string> _source = new List<string>();
        private MudCarousel<string> _carousel;

        private bool isBigThreeRoles = false;
        private bool isViewOnly = false;
        private bool isCashier = false;
        private bool isTechnician = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isCashier = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            isTechnician = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.SeniorTechnician)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.JuniorTechnician);

            #region Initialize Request Model
            InspectionRequestModel = new InspectionDTO()
            {
                Customer = new CustomerDTO(),
                InspectorUser = new UserDTO(),
                JobStatus = new JobStatusDTO(),
                Vehicle = new VehicleDTO()
                {
                    VehicleModel = new VehicleModelDTO()
                    {
                        VehicleMake = new VehicleMakeDTO()
                    }
                }
            };
            #endregion

            // Load the inspection template
            string text = FileHelper.GetInspectionTemplate();
            var inpectionListFromTemplate = JsonConvert.DeserializeObject<IList<InspectionGroupModel>>(text);

            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);

            var inspectionTemplateSequenced = new List<InspectionGroupModel>();
            foreach (var it in inpectionListFromTemplate)
            {
                if (it.Group.ToUpper() == "LIGHTS")
                    it.Sequence = 1;
                if (it.Group.ToUpper() == "FLUIDS")
                    it.Sequence = 2;
                if (it.Group.ToUpper() == "PMS")
                    it.Sequence = 3;
                if (it.Group.ToUpper() == "BATTERY")
                    it.Sequence = 4;
                if (it.Group.ToUpper() == "BELTS")
                    it.Sequence = 5;
                if (it.Group.ToUpper() == "BRAKES")
                    it.Sequence = 6;
                if (it.Group.ToUpper() == "TIRES")
                    it.Sequence = 7;
                if (it.Group.ToUpper() == "DRIVE TRAIN")
                    it.Sequence = 8;
                if (it.Group.ToUpper() == "SUSPENSIONS")
                    it.Sequence = 9;

                inspectionTemplateSequenced.Add(it);
            }


            InspectionTemplate = inspectionTemplateSequenced.OrderBy(x => x.Sequence).ToList();

            var userList = await UserService.GetAllUserRoleAsync();
            var userWithRoles = new List<UserDTO>();

            foreach (var u in userList)
            {
                var userInfo = new UserDTO();

                userInfo = u;
                userInfo.UserRoles = await UserRolesService.GetUserRolesByUserIdAsync(u.Id);
            }

            IsEditMode = !string.IsNullOrEmpty(InspectionId);

            CustomerList = await CustomerService.GetAllAsync();
            InspectorList = userList.Where(x => x.IsActive && x.UserRoles.Any(x => x.Role.Name.Equals(Constants.UserRoles.ServiceAdvisor))).ToList();
            JobStatusList = await JobStatusService.GetAllAsync();

            ServiceAdvisorList = userList.Where(x => x.IsActive && x.UserRoles.Any(x => x.Role.Name.Equals(Constants.UserRoles.ServiceAdvisor))).ToList();
            EstimatorList = userList.Where(x => x.IsActive && x.UserRoles.Any(x => x.Role.Name.Equals(Constants.UserRoles.Estimator))).ToList();
            ApproverList = userList.Where(x => x.IsActive && x.UserRoles.Any(x => x.Role.Name.Equals(Constants.UserRoles.Supervisor))).ToList();
            ServiceGroupList = await ServiceGroupService.GetAllAsync();
            
            if (IsEditMode)
            {
                InspectionRequestModel = await InspectionService.GetInspectionByIdAsync(int.Parse(InspectionId));
                JobStatusName = InspectionRequestModel.JobStatus.Name;

                // creteria of locked based on status.
                isInspectionLocked = 
                    InspectionRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Converted) ||
                    InspectionRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Completed) ||
                    InspectionRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Cancelled) ||
                    isViewOnly;

                form.Disabled = isInspectionLocked;
                IsEditMode = !isInspectionLocked;

                InspectionRequestModel.TechnicianList = await InspectionTechnicianService.GetAllInspectionTechnicianByInspectionIdAsync(InspectionRequestModel.Id);

                InspectionTemplate = JsonConvert.DeserializeObject<IList<InspectionGroupModel>>(InspectionRequestModel.InspectionDetails).ToList();

                var inspectionTemplateEditSequenced = new List<InspectionGroupModel>();
                foreach (var it in InspectionTemplate)
                {
                    if (it.Group.ToUpper() == "LIGHTS")
                        it.Sequence = 1;
                    if (it.Group.ToUpper() == "FLUIDS")
                        it.Sequence = 2;
                    if (it.Group.ToUpper() == "PMS")
                        it.Sequence = 3;
                    if (it.Group.ToUpper() == "BATTERY")
                        it.Sequence = 4;
                    if (it.Group.ToUpper() == "BELTS")
                        it.Sequence = 5;
                    if (it.Group.ToUpper() == "BRAKES")
                        it.Sequence = 6;
                    if (it.Group.ToUpper() == "TIRES")
                        it.Sequence = 7;
                    if (it.Group.ToUpper() == "DRIVE TRAIN")
                        it.Sequence = 8;
                    if (it.Group.ToUpper() == "SUSPENSIONS")
                        it.Sequence = 9;

                    inspectionTemplateEditSequenced.Add(it);
                }


                InspectionTemplate = inspectionTemplateEditSequenced.OrderBy(x => x.Sequence).ToList();

                InspectionTechnicians = InspectionRequestModel.TechnicianList == null
                    ? new List<InspectionTechnicianDTO>()
                    : InspectionRequestModel.TechnicianList;

                // Create folder if not exist.
                var uploadDirectory = Path.Combine(_environment.WebRootPath, "Inspections", InspectionRequestModel.ReferenceNo);
                if (!Directory.Exists(uploadDirectory))
                    Directory.CreateDirectory(uploadDirectory);

                GetFilesByReferenceNo(InspectionRequestModel.ReferenceNo);
            }
            else
            {
                CustomerName = "New Inspection";

                InspectionRequestModel.JobStatus = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                InspectionRequestModel.TransactionDate = DateTime.Now;
                InspectionRequestModel.ExpirationDate = DateTime.Now.AddMonths(1);

                var authState = await AuthState;
                var advisorUser = userList.Where(x => x.Id == TokenHelper.GetCurrentUserId(authState));

                InspectionRequestModel.AdvisorUser = isTechnician
                    ? null
                    : advisorUser.FirstOrDefault();

                JobStatusName = InspectionRequestModel.JobStatus.Name;

                InspectionRequestModel.ReferenceNo = await ReferenceNumberHelper.GetRNInspection(InspectionService);
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task OnReOpenClick()
        {
            if (string.IsNullOrEmpty(InspectionId))
                return;

            bool? result = await mbox.ShowAsync();
            var proceed = result == null ? false : true;

            if (proceed)
            {
                IsLoading = true;

                var jobStatusOpen = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                InspectionRequestModel.JobStatus = jobStatusOpen;
                InspectionRequestModel.JobStatusId = jobStatusOpen.Id;

                await InspectionService.UpdateAsync(InspectionRequestModel);
                SnackbarService.Add("Inspection Successfuly re-OPENED!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                IsLoading = false;
                StateHasChanged();

                NavigationManager.NavigateToCustom("/operations/inspections", true);
            }
        }

        private void OnRedClick(InspectionDetailsModel model)
        {
            if (model.IsRed == true)
            {
                model.IsRed = false;
                model.IsAmber = false;
                model.IsGreen = false;
            }
            else
            {
                model.IsRed = true;
                model.IsAmber = false;
                model.IsGreen = false;
            }
        }

        private void OnAmberClick(InspectionDetailsModel model)
        {
            if (model.IsAmber == true)
            {
                model.IsRed = false;
                model.IsAmber = false;
                model.IsGreen = false;
            }
            else
            {
                model.IsRed = false;
                model.IsAmber = true;
                model.IsGreen = false;
            }
        }

        private void OnGreenClick(InspectionDetailsModel model)
        {
            if (model.IsGreen == true)
            {
                model.IsRed = false;
                model.IsAmber = false;
                model.IsGreen = false;
            }
            else
            {
                model.IsRed = false;
                model.IsAmber = false;
                model.IsGreen = true;
            }
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
                var uploadDirectory = Path.Combine(_environment.WebRootPath, "Inspections", InspectionRequestModel.ReferenceNo);
                if (!Directory.Exists(uploadDirectory))
                    Directory.CreateDirectory(uploadDirectory);

                var jsonData = JsonConvert.SerializeObject(InspectionTemplate);

                try
                {
                    IsLoading = true;
                    bool isEditMode = !string.IsNullOrEmpty(InspectionId);

                    InspectionRequestModel.CustomerId = InspectionRequestModel.Customer.Id;
                    InspectionRequestModel.VehicleId = InspectionRequestModel.Vehicle.Id;
                    InspectionRequestModel.JobStatusId = InspectionRequestModel.JobStatus.Id;
                    InspectionRequestModel.InspectorUserId = TokenHelper.GetCurrentUserId(await AuthState);

                    InspectionRequestModel.AdvisorUserId = InspectionRequestModel.AdvisorUser.Id;
                    InspectionRequestModel.ApproverUserId = 12; // temp /*InspectionRequestModel.ApproverUser.Id*/;
                    InspectionRequestModel.ServiceGroupId = InspectionRequestModel.ServiceGroup.Id;

                    InspectionRequestModel.TechnicianList = InspectionTechnicians;

                    var authState = await AuthState;
                    InspectionRequestModel.EstimatorUserId = TokenHelper.GetCurrentUserId(authState);

                    if (!isEditMode) // create mode
                    {
                        InspectionRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        InspectionRequestModel.CreatedDateTime = DateTime.Now;
                        InspectionRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        InspectionRequestModel.UpdatedDateTime = DateTime.Now;

                        InspectionRequestModel.InspectionDetails = jsonData;

                        // call create endpoint here...
                        var created = await InspectionService.CreateAsync(InspectionRequestModel);

                        // Save technicians
                        foreach (var t in InspectionRequestModel.TechnicianList)
                        {
                            t.InspectionId = created.Id;
                            t.TechnicianUserId = t.TechnicianUser.Id;
                            t.InspectionId = t.InspectionId;

                            t.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            t.CreatedDateTime = DateTime.Now;
                            t.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            t.UpdatedDateTime = DateTime.Now;

                            await InspectionTechnicianService.CreateAsync(t);
                        }

                        InspectionId = created.Id.ToString();
                        //await ReloadEstimateData();

                        IsLoading = false;

                        SnackbarService.Add("Inspection Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/inspections", true);
                    }
                    else // update mode
                    {
                        int inspectionId = int.Parse(InspectionId);

                        InspectionRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        InspectionRequestModel.UpdatedDateTime = DateTime.Now;

                        InspectionRequestModel.EstimatorUserId = TokenHelper.GetCurrentUserId(authState);

                        InspectionRequestModel.InspectionDetails = jsonData;

                        var inspectionTechnicianList = await InspectionTechnicianService.GetAllInspectionTechnicianByInspectionIdAsync(inspectionId);
                        if (inspectionTechnicianList != null && inspectionTechnicianList.Any())
                        {
                            foreach (var del in inspectionTechnicianList)
                            {
                                await InspectionTechnicianService.DeleteAsync(del.Id);
                            }

                        }

                        // Detele all current Technicians? Update also include insert inside
                        // Save Technicians -> need to check for logic if the Technician is deleted.
                        foreach (var t in InspectionRequestModel.TechnicianList)
                        {
                            t.TechnicianUserId = t.TechnicianUser.Id;
                            t.InspectionId = inspectionId;
                            t.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            t.CreatedDateTime = DateTime.Now;
                            t.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            t.UpdatedDateTime = DateTime.Now;
                            await InspectionTechnicianService.CreateAsync(t);
                        }

                        // call update endpoint here...
                        await InspectionService.UpdateAsync(InspectionRequestModel);

                        //await ReloadEstimateData();

                        SnackbarService.Add("Inspection Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/inspections", true);
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

        private async Task OnCancelClick()
        {
            NavigationManager.NavigateToCustom("/operations/inspections");
        }

        private async Task<bool> ValidateSubComponents()
        {
            if (!InspectionTechnicians.Any())
            {
                mBoxCustomMessage = "Technician is required!";
                await mboxError.ShowAsync();

                return false;
            }

            if (InspectionTechnicians.Count() < 1)
            {
                mBoxCustomMessage = "Please select at lease one (1) technician!";
                await mboxError.ShowAsync();

                return false;
            }

            return true;
        }

        private async Task OnConvertToEstimate()
        {
            var isValidated = await ValidateSubComponents();

            if (!isValidated)
                return;

            if (InspectionRequestModel.TechnicianList == null || !InspectionRequestModel.TechnicianList.Any())
            {
                mBoxCustomMessage = "Technician is not updated. Please save the record first.";
                await mboxError.ShowAsync();

                return;
            }

            mBoxCustomMessage = "Are you sure you want to move this inpection to estimate?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                IsLoading = true;

                var EstimateRequestModel = new EstimateDTO();
                var jobStatusOpen = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();

                EstimateRequestModel.ReferenceNo = await ReferenceNumberHelper.GetRNEstimate(EstimateService);

                EstimateRequestModel.EstimatedDays = 0;
                EstimateRequestModel.IsPackage = false;
                EstimateRequestModel.IsCustomerApproved = false;
                EstimateRequestModel.JobStatusId = jobStatusOpen.Id;
                EstimateRequestModel.TransactionDate = DateTime.Now;
                EstimateRequestModel.ExpirationDate = DateTime.Now.AddMonths(1);
                //EstimateRequestModel.Customer = InspectionRequestModel.Customer;
                EstimateRequestModel.CustomerId = InspectionRequestModel.Customer.Id;
                //EstimateRequestModel.Vehicle = InspectionRequestModel.Vehicle;
                EstimateRequestModel.VehicleId = InspectionRequestModel.Vehicle.Id;
                EstimateRequestModel.ServiceGroupId = InspectionRequestModel.ServiceGroup.Id;
                EstimateRequestModel.Odometer = InspectionRequestModel.Odometer;
                EstimateRequestModel.AdvisorUserId = InspectionRequestModel.AdvisorUser.Id;
               
                EstimateRequestModel.ApproverUserId = InspectionRequestModel.ApproverUser.Id;

                var authState = await AuthState;
                EstimateRequestModel.EstimatorUserId = TokenHelper.GetCurrentUserId(authState);

                EstimateRequestModel.CustomerPO = string.Empty;
                EstimateRequestModel.Summary = string.Empty;
                EstimateRequestModel.SubTotal = 0;
                EstimateRequestModel.VAT12 = 0;
                EstimateRequestModel.LaborDiscount = 0;
                EstimateRequestModel.ProductDiscount = 0;
                EstimateRequestModel.AdditionalDiscount = 0;
                EstimateRequestModel.TotalAmount = 0;

                EstimateRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                EstimateRequestModel.CreatedDateTime = DateTime.Now;
                EstimateRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                EstimateRequestModel.UpdatedDateTime = DateTime.Now;

                EstimateRequestModel.Inspectionid = InspectionRequestModel.Id;

                var created = await EstimateService.CreateAsync(EstimateRequestModel);

                EstimateRequestModel.TechnicianList = new List<EstimateTechnicianDTO>();
                foreach (var t in InspectionRequestModel.TechnicianList)
                {
                    EstimateRequestModel.TechnicianList.Add(new EstimateTechnicianDTO()
                    {
                        EstimateId = created.Id,
                        TechnicianUserId = t.TechnicianUserId
                    });
                }

                // Save technicians
                foreach (var t in EstimateRequestModel.TechnicianList)
                {
                    t.EstimateId = created.Id;
                    t.TechnicianUserId = t.TechnicianUserId;

                    t.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                    t.CreatedDateTime = DateTime.Now;
                    t.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                    t.UpdatedDateTime = DateTime.Now;

                    await EstimateTechnicianService.CreateAsync(t);
                }

                // update estimate status to converted
                var convertedStatus = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Converted)).FirstOrDefault();
                InspectionRequestModel.JobStatus = convertedStatus;
                InspectionRequestModel.JobStatusId = convertedStatus.Id;

                await InspectionService.UpdateAsync(InspectionRequestModel);

                IsLoading = false;
                StateHasChanged();

                SnackbarService.Add("Estimate Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                NavigationManager.NavigateToCustom($"/operations/estimates/{created.Id}", true);
            }
        }

        private async Task OnGeneratePdfClick()
        {
            IsLoading = true;

            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            var inspectionData = await InspectionService.GetInspectionByIdAsync(InspectionRequestModel.Id);
            var inspectionTemplate = JsonConvert.DeserializeObject<IList<InspectionGroupModel>>(inspectionData.InspectionDetails).ToList();

            var inspectionTemplateSequenced = new List<InspectionGroupModel>();
            foreach (var it in inspectionTemplate)
            {
                if (it.Group.ToUpper() == "LIGHTS")
                    it.Sequence = 1;
                if (it.Group.ToUpper() == "FLUIDS")
                    it.Sequence = 2;
                if (it.Group.ToUpper() == "PMS")
                    it.Sequence = 3;
                if (it.Group.ToUpper() == "BATTERY")
                    it.Sequence = 4;
                if (it.Group.ToUpper() == "BELTS")
                    it.Sequence = 5;
                if (it.Group.ToUpper() == "BRAKES")
                    it.Sequence = 6;
                if (it.Group.ToUpper() == "TIRES")
                    it.Sequence = 7;
                if (it.Group.ToUpper() == "DRIVE TRAIN")
                    it.Sequence = 8;
                if (it.Group.ToUpper() == "SUSPENSIONS")
                    it.Sequence = 9;

                inspectionTemplateSequenced.Add(it);
            }

            var technicians = await InspectionTechnicianService.GetAllInspectionTechnicianByInspectionIdAsync(InspectionRequestModel.Id);
            inspectionData.TechnicianList = technicians.Where(x => x.TechnicianUser.Role.Name == "SENIOR TECHNICIAN").ToList();

            InspectionReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            InspectionReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
            await InspectionReportGenerator.Generate(inspectionData, JSRuntime, inspectionTemplateSequenced, companyData);

            IsLoading = false;
        }

        private async Task OnNewEstimateClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the current transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
                NavigationManager.NavigateToCustom("/operations/inspections/add", true);
        }

        private async Task OnCancelEstimateClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the this transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                var jobStatusCancelled = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Cancelled)).FirstOrDefault();
                InspectionRequestModel.JobStatus = jobStatusCancelled;
                InspectionRequestModel.JobStatusId = jobStatusCancelled.Id;

                await InspectionService.UpdateAsync(InspectionRequestModel);

                SnackbarService.Add("Inspection Successfuly Cancelled!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                NavigationManager.NavigateToCustom("/operations/inspections", true);
            }
        }

        private async Task OnCustomerChanged(InspectionDTO dto, CustomerDTO i)
        {
            InspectionRequestModel.Customer = i;
            var vehicleByCustomer = await VehicleService.GetAllVehicleByCustomerIdAsync(i.Id);

            VehicleList = vehicleByCustomer;

            StateHasChanged();
        }

        private void OnAddNewItemClick(EstimateDialogType dialogType)
        {
            bool isEditMode = !string.IsNullOrEmpty(InspectionId);
            var returnUrl = isEditMode
                ? $"/operations/inspections/{InspectionId}"
                : "/operations/inspections/add";

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

        private async Task<IEnumerable<UserDTO>> SearchInspector(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return InspectorList;

            return InspectorList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<JobStatusDTO>> SearchJobStatus(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return JobStatusList;

            return JobStatusList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
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

        private async Task<IEnumerable<UserDTO>> SearchServiceAdvisor(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ServiceAdvisorList;

            return ServiceAdvisorList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
        #endregion

        private void OpenFile(string filename)
        {
            imageSource = $"/Inspections/{InspectionRequestModel.ReferenceNo}/{Path.GetFileName(filename)}";
            StateHasChanged();
        }

        private async Task OnFileDeleteClick(string path)
        {
            var fileToDelete = Path.Combine(_environment.WebRootPath, "Inspections", InspectionRequestModel.ReferenceNo, Path.GetFileName(path));
            if (System.IO.File.Exists(fileToDelete))
                System.IO.File.Delete(fileToDelete);

            uploadedFiles = new List<string>();
            GetFilesByReferenceNo(InspectionRequestModel.ReferenceNo);
            StateHasChanged();
        }

        private void GetFilesByReferenceNo(string referenceNo)
        {
            var filesDirectory = Path.Combine(_environment.WebRootPath, "Inspections", referenceNo);
            
            string[] fileEntries = Directory.GetFiles(filesDirectory);

            uploadedFiles = new List<string>();
            _source = new List<string>();
            foreach (string fileName in fileEntries)
            {
                var fName = $"/Inspections/{referenceNo}/{Path.GetFileName(fileName)}";
                _source.Add(fName);
                uploadedFiles.Add(fName);
            }
        }

        private async Task UploadFiles(InputFileChangeEventArgs args)
        {
            try
            {
                // TODO: Implement validation here...

                files.Add(args.File);

                var uploadDirectory = Path.Combine(_environment.WebRootPath, "Inspections", InspectionRequestModel.ReferenceNo);
                if (!Directory.Exists(uploadDirectory))
                    Directory.CreateDirectory(uploadDirectory);

                var fileName = $"{Guid.NewGuid()}-{args.File.Name}";
                var path = Path.Combine(uploadDirectory, fileName.Replace(" ", "_"));


                await using var fs = new FileStream(path, FileMode.Create);
                await args.File.OpenReadStream(9512000).CopyToAsync(fs);

                GetFilesByReferenceNo(InspectionRequestModel.ReferenceNo);
                StateHasChanged();
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
}