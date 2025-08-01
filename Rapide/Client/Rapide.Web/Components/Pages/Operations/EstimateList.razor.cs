using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.PdfReportGenerator;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class EstimateList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IEstimateService EstimateService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private IEstimatePackageService EstimatePackageService { get; set; }
        [Inject]
        private IEstimateServiceService EstimateServiceService { get; set; }
        [Inject]
        private IEstimateProductService EstimateProductService { get; set; }
        [Inject]
        private IEstimateTechnicianService EstimateTechnicianService { get; set; }
        [Inject]
        private IPackageService PackageService { get; set; }
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

        private MudDataGrid<EstimateModel> dataGrid;
        private string searchString;
        private List<EstimateModel> EstimateRequestModel = new List<EstimateModel>();

        private bool isCashier = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            isCashier = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Cashier);

            JobStatusList = await JobStatusService.GetAllAsync();

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task ReloadRequestModel()
        {
            try
            {

                var dataList = await EstimateService.GetAllEstimateAsync();

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

                    EstimateRequestModel.Add(new EstimateModel()
                    {
                        IsAllowedToOverride = TokenHelper.IsBigThreeRoles(await AuthState),
                        StatusChipColor = statusColor,
                        Id = ul.Id,
                        ReferenceNo = ul.ReferenceNo,
                        Customer = customerMap,
                        Vehicle = new VehiclesModel()
                        {
                            Id = ul.Vehicle.Id,
                            VehicleModel = vehicleModelMap,
                            PlateNo = ul.Vehicle.PlateNo,
                            YearModel = ul.Vehicle.YearModel
                        },
                        TransactionDate = ul.TransactionDate,
                        JobStatus = jobStatusMap
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

        private async Task<GridData<EstimateModel>> ServerReload(GridState<EstimateModel> state)
        {
            if (!EstimateRequestModel.Any())
                await ReloadRequestModel();

            IEnumerable<EstimateModel> data = new List<EstimateModel>();
            data = EstimateRequestModel.OrderByDescending(x => x.TransactionDate);

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
                if (element.Vehicle.PlateNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.TransactionDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
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
                    case nameof(EstimateModel.ReferenceNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(EstimateModel.TransactionDate):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(EstimateModel.Customer.FirstName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Customer.FirstName
                        );
                        break;
                    case nameof(EstimateModel.Vehicle):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Vehicle.VehicleModel.VehicleMake.Name
                        );
                        break;
                    case nameof(EstimateModel.Vehicle.PlateNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Vehicle.PlateNo
                        );
                        break;

                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<EstimateModel>
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

        private void OnAddClick()
        {
            NavigationManager.NavigateToCustom("/operations/estimates/add");
        }

        private async Task OnDeleteClick(EstimateModel model)
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

                        var dataToDelete = await EstimateService.GetEstimateByIdAsync(model.Id);
                        dataToDelete.JobStatus = jobStatusDeleted;
                        dataToDelete.JobStatusId = jobStatusDeleted.Id;

                        await EstimateService.UpdateAsync(dataToDelete);

                        SnackbarService.Add("Estimate Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/operations/estimates", true);
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

        private async Task OnGeneratePdfClick(int estimateId)
        {
            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            var EstimateRequestModel = await EstimateService.GetEstimateByIdAsync(estimateId);

            if (EstimateRequestModel == null)
                return;

            EstimateRequestModel.ProductList = await EstimateProductService.GetAllEstimateProductByEstimateIdAsync(EstimateRequestModel.Id);
            EstimateRequestModel.ServiceList = await EstimateServiceService.GetAllEstimateServiceByEstimateIdAsync(EstimateRequestModel.Id);
            
            var technicians = await EstimateTechnicianService.GetAllEstimateTechnicianByEstimateIdAsync(EstimateRequestModel.Id);
            EstimateRequestModel.TechnicianList = technicians.Where(x => x.TechnicianUser.Role.Name == "SENIOR TECHNICIAN").ToList();


            EstimateRequestModel.PackageList = await EstimatePackageService.GetAllEstimatePackageByEstimateIdAsync(EstimateRequestModel.Id);

            EstimateReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            EstimateReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
            await EstimateReportGenerator.Generate(EstimateRequestModel, JSRuntime, companyData);
        }
    }
}