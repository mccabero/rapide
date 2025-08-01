using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Services;
using Rapide.Web.Components.Pages.SystemConfiguration;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.PdfReportGenerator;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class InspectionList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }

        [Inject]
        private IInspectionService InspectionService { get; set; }
        [Inject]
        private IInspectionTechnicianService InspectionTechnicianService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        [Inject]
        private IJobStatusService JobStatusService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mboxCustom { get; set; }
        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private List<JobStatusDTO> JobStatusList { get; set; } = new();

        private MudDataGrid<InspectionModel> dataGrid;
        private string searchString;
        private List<InspectionModel> InspectionRequestModel = new List<InspectionModel>();
        private bool isViewOnly = false;
        private bool isCashier = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isCashier = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier)
                 || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            JobStatusList = await JobStatusService.GetAllAsync();

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task ReloadRequestModel()
        {
            try
            {
                var dataList = await InspectionService.GetAllInspectionSummaryAsync();

                if (dataList == null)
                {
                    IsLoading = false;
                    return;
                }

                IMapper mapper = MappingWebHelper.InitializeMapper();

                foreach (var ul in dataList)
                {
                    Color statusColor = Color.Primary;
                    if (ul.JobStatus.Name.Equals(Constants.JobStatus.Open))
                        statusColor = Color.Warning;
                    else if (ul.JobStatus.Name.Equals(Constants.JobStatus.Converted))
                        statusColor = Color.Success;
                    else if (ul.JobStatus.Name.Equals(Constants.JobStatus.Cancelled))
                        statusColor = Color.Info;
                    else if (ul.JobStatus.Name.Equals(Constants.JobStatus.Deleted))
                        statusColor = Color.Error;

                    var customerMap = mapper.Map<CustomerModel>(ul.Customer);
                    var vehicleModelMap = mapper.Map<VehicleModelModel>(ul.Vehicle.VehicleModel);
                    var jobStatusMap = ul.JobStatus.Map<JobStatusModel>();

                    InspectionRequestModel.Add(new InspectionModel()
                    {
                        IsAllowedToOverride = TokenHelper.IsBigThreeRoles(await AuthState),
                        StatusChipColor = statusColor,
                        Id = ul.Id,
                        ReferenceNo = ul.ReferenceNo,
                        TransactionDate = ul.TransactionDate,
                        Customer = customerMap,
                        JobStatus = jobStatusMap,
                        Vehicle = new VehiclesModel()
                        {
                            Id = ul.Vehicle.Id,
                            VehicleModel = vehicleModelMap,
                            PlateNo = ul.Vehicle.PlateNo,
                            YearModel = ul.Vehicle.YearModel
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                StateHasChanged();

                throw new Exception(ex.Message);
            }
        }

        private void OnAddClick()
        {
            NavigationManager.NavigateToCustom("/operations/inspections/add");
        }

        private async Task<GridData<InspectionModel>> ServerReload(GridState<InspectionModel> state)
        {
            if (!InspectionRequestModel.Any())
                await ReloadRequestModel();

            IEnumerable<InspectionModel> data = new List<InspectionModel>();
            data = InspectionRequestModel.OrderByDescending(x => x.TransactionDate);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.ReferenceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Customer.FirstName} {element.Customer.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Vehicle.VehicleModel.VehicleMake.Name} {element.Vehicle.VehicleModel.Name} {element.Vehicle.YearModel}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.TransactionDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Vehicle.PlateNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.JobStatus.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.JobStatus.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(InspectionModel.Id):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(InspectionModel.Customer.FirstName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Customer.FirstName
                        );
                        break;
                    case nameof(InspectionModel.TransactionDate):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.TransactionDate
                        );
                        break;
                    case nameof(InspectionModel.Vehicle.PlateNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Vehicle.PlateNo
                        );
                        break;

                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<InspectionModel>
            {
                TotalItems = totalItems,
                Items = pagedData
            };
        }

        private Task OnSearch(string text)
        {
            searchString = text;
            return dataGrid.ReloadServerData();
        }

        private async Task OnDeleteClick(InspectionModel model)
        {
            try
            {
                if (model != null)
                {
                    // validate if model status is not OPEN then prevent deletion
                    if (!model.JobStatus.Name.Equals(Constants.JobStatus.Open))
                    {
                        mBoxCustomMessage = "Completed / Converted estimate cannot be deleted.";
                        await mboxError.ShowAsync();

                        return;
                    }

                    bool? result = await mbox.ShowAsync();
                    var proceed = result == null ? false : true;

                    if (proceed)
                    {
                        IsLoading = true;

                        var jobStatusDeleted = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Deleted)).FirstOrDefault();

                        var dataToDelete = await InspectionService.GetInspectionByIdAsync(model.Id);
                        dataToDelete.JobStatus = jobStatusDeleted;
                        dataToDelete.JobStatusId = jobStatusDeleted.Id;

                        await InspectionService.UpdateAsync(dataToDelete);

                        SnackbarService.Add("Inspection Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/operations/inspections", true);
                    }
                }
            }
            catch (Exception)
            {
                mBoxCustomMessage = "Unable to delete the this record. This might be used in another transaction.";
                await mboxError.ShowAsync();

                IsLoading = false;
                StateHasChanged();
                return;
            }
        }

        private async Task OnGeneratePdfClick(int inspectionId)
        {
            IsLoading = true;

            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            var inspectionData = await InspectionService.GetInspectionByIdAsync(inspectionId);
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

            var technicians = await InspectionTechnicianService.GetAllInspectionTechnicianByInspectionIdAsync(inspectionId);
            inspectionData.TechnicianList = technicians.Where(x => x.TechnicianUser.Role.Name == "SENIOR TECHNICIAN").ToList();

            InspectionReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            InspectionReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
            await InspectionReportGenerator.Generate(inspectionData, JSRuntime, inspectionTemplateSequenced, companyData);

            IsLoading = false;
        }
    }
}