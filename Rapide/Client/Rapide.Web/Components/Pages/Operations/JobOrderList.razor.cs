using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.PdfReportGenerator;
using static MudBlazor.CategoryTypes;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class JobOrderList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IJobOrderService JobOrderService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private IJobOrderServiceService JobOrderServiceService { get; set; }
        [Inject]
        private IJobOrderProductService JobOrderProductService { get; set; }
        [Inject]
        private IJobOrderPackageService JobOrderPackageService { get; set; }
        [Inject]
        private IJobOrderTechnicianService JobOrderTechnicianService { get; set; }
        [Inject]
        private IPackageService PackageService { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        [Inject]
        private IJobStatusService JobStatusService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private MudDataGrid<JobOrderModel> dataGrid;
        private string searchString;
        private List<JobOrderModel> JobOrderRequestModel = new List<JobOrderModel>();

        private List<JobStatusDTO> JobStatusList { get; set; } = new();
        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }
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
                var dataList = await JobOrderService.GetAllJobOrderAsync();

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

                    JobOrderRequestModel.Add(new JobOrderModel()
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

        private async Task<GridData<JobOrderModel>> ServerReload(GridState<JobOrderModel> state)
        {
            if (!JobOrderRequestModel.Any())
                await ReloadRequestModel();

            IEnumerable<JobOrderModel> data = new List<JobOrderModel>();
            data = JobOrderRequestModel.OrderByDescending(x => x.TransactionDate);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.ReferenceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Customer.FirstName} {element.Customer.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.TransactionDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Vehicle.VehicleModel.VehicleMake.Name} {element.Vehicle.VehicleModel.Name} {element.Vehicle.YearModel}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Vehicle.PlateNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
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
                    case nameof(JobOrderModel.ReferenceNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.ReferenceNo
                        );
                        break;
                    case nameof(JobOrderModel.Customer):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Customer.FirstName
                        );
                        break;
                    case nameof(JobOrderModel.TransactionDate):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.TransactionDate
                        );
                        break;
                    case nameof(JobOrderModel.Vehicle):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Vehicle.VehicleModel.VehicleMake.Name
                        );
                        break;

                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<JobOrderModel>
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
            NavigationManager.NavigateToCustom("/operations/job-orders/add");
        }

        private async Task OnDeleteClick(JobOrderModel model)
        {
            try
            {
                // validate if model status is not OPEN then prevent deletion
                if (!model.JobStatus.Name.Equals(Constants.JobStatus.Open))
                {
                    mBoxCustomMessage = "Completed / Converted job order cannot be deleted.";
                    await mboxError.ShowAsync();

                    return;
                }

                if (model != null)
                {
                    bool? result = await mbox.ShowAsync();
                    var proceed = result == null ? false : true;

                    if (proceed)
                    {
                        IsLoading = true;

                        var jobStatusDeleted = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Deleted)).FirstOrDefault();

                        var dataToDelete = await JobOrderService.GetJobOrderByIdAsync(model.Id);
                        dataToDelete.JobStatus = jobStatusDeleted;
                        dataToDelete.JobStatusId = jobStatusDeleted.Id;

                        await JobOrderService.UpdateAsync(dataToDelete);

                        SnackbarService.Add("Job Order Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/operations/job-orders", true);
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

        private async Task OnGeneratePdfClick(int jobOrderId)
        {
            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            var JobOrderRequestModel = await JobOrderService.GetJobOrderByIdAsync(jobOrderId);

            if (JobOrderRequestModel == null)
                return;

            JobOrderRequestModel.ProductList = await JobOrderProductService.GetAllJobOrderProductByJobOrderIdAsync(JobOrderRequestModel.Id);
            JobOrderRequestModel.ServiceList = await JobOrderServiceService.GetAllJobOrderServiceByJobOrderIdAsync(JobOrderRequestModel.Id);
            JobOrderRequestModel.PackageList = await JobOrderPackageService.GetAllJobOrderPackageByJobOrderIdAsync(JobOrderRequestModel.Id);

            var technicians = await JobOrderTechnicianService.GetAllJobOrderTechnicianByJobOrderIdAsync(JobOrderRequestModel.Id);
            JobOrderRequestModel.TechnicianList = technicians.Where(x => x.TechnicianUser.Role.Name == "SENIOR TECHNICIAN").ToList();

            JobOrderReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            JobOrderReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
            await JobOrderReportGenerator.Generate(JobOrderRequestModel, JSRuntime, companyData);
        }
    }
}
