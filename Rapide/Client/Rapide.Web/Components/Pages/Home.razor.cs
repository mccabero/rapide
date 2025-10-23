using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Pages.Customers;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using System.Threading.Tasks;
using static Rapide.Web.Components.Utilities.Constants;
using System.Linq;

namespace Rapide.Web.Components.Pages
{
    public partial class Home
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }

        [Inject]
        private IUserService UserServices { get; set; }
        [Inject]
        private ICustomerService CustomerServices { get; set; }
        [Inject]
        private IJobOrderService JobOrderService { get; set; }
        #endregion

        #region Private Properties
        private bool isTechnician = false;

        private List<UserDTO> userList = new();
        private List<CustomerDTO> customerList = new();

        private List<UserDTO> EmployeeBirthdayMonthList = new();
        private List<CustomerDTO> CustomerBirthdayMonthList = new();

        private MudDataGrid<JobOrderModel> dataGrid;
        private string searchString;
        private List<JobOrderModel> JobOrderRequestModel = new List<JobOrderModel>();

        private bool IsLoading { get; set; }
        // 1: Rapide | 2: Changan | 3: ALL
        public string clientType { get; set; } = Constants.ClientType.All;
        private string clientTypeFilter;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            isTechnician = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.SeniorTechnician)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.JuniorTechnician);

            userList = await UserServices.GetAllUserRoleAsync();
            customerList = await CustomerServices.GetAllAsync();

            var employeeBirthdayMonth = userList
                .Where(x => x.Birthday.HasValue && x.Birthday.Value.Month == DateTime.Now.Month && x.Birthday.Value.Day >= DateTime.Now.Day)
                .OrderBy(x => x.Birthday.Value.Day)
                .ToList();

            var customerBirhtdayMonth = customerList
                .Where(x => x.Birthday.HasValue && x.Birthday.Value.Month == DateTime.Now.Month && x.Birthday.Value.Day >= DateTime.Now.Day)
                .OrderBy(x => x.Birthday.Value.Day)
                .ToList();

            foreach (var ebm in employeeBirthdayMonth)
            {
                if (!EmployeeBirthdayMonthList.Any(x => x.Id == ebm.Id))
                {
                    EmployeeBirthdayMonthList.Add(ebm);
                }
            }

            foreach (var cbm in customerBirhtdayMonth)
            {
                if (!CustomerBirthdayMonthList.Any(x => x.Id == cbm.Id))
                {
                    CustomerBirthdayMonthList.Add(cbm);
                }
            }

            await base.OnInitializedAsync();
        }

        #region Job Orders
        private async Task ReloadRequestModel()
        {
            try
            {
                IsLoading = true;

                var dataList = await JobOrderService.GetAllJobOrderSummaryAsync();

                if (dataList == null || !dataList.Any())
                {
                    JobOrderRequestModel.Clear();
                    IsLoading = false;
                    return;
                }

                var sixMonthsAgo = DateTime.Now.AddMonths(-6);

                // Filter on server ideally; do minimal client filtering and efficient projection.
                var filtered = dataList
                    .Where(x => x.CreatedDateTime > sixMonthsAgo)
                    .OrderByDescending(x => x.TransactionDate)
                    .ToList();

                // Initialize mapper once
                IMapper mapper = MappingWebHelper.InitializeMapper();

                // Determine allowed override once per current user
                var isAllowedToOverride = TokenHelper.IsBigThreeRoles(await AuthState);

                // Project to JobOrderModel using LINQ (faster than manual add in loop)
                var projected = filtered.Select(ul =>
                {
                    Color statusColor = Color.Primary;
                    if (ul.JobStatus?.Name.Equals(Constants.JobStatus.Open) == true)
                        statusColor = Color.Warning;
                    else if (ul.JobStatus?.Name.Equals(Constants.JobStatus.Converted) == true)
                        statusColor = Color.Success;
                    else if (ul.JobStatus?.Name.Equals(Constants.JobStatus.Cancelled) == true)
                        statusColor = Color.Info;
                    else if (ul.JobStatus?.Name.Equals(Constants.JobStatus.Deleted) == true)
                        statusColor = Color.Error;

                    var customerMap = ul.Customer != null ? mapper.Map<CustomerModel>(ul.Customer) : new CustomerModel();
                    var vehicleModelMap = ul.Vehicle?.VehicleModel != null ? mapper.Map<VehicleModelModel>(ul.Vehicle.VehicleModel) : new VehicleModelModel();
                    var jobStatusMap = ul.JobStatus != null ? ul.JobStatus.Map<JobStatusModel>() : new JobStatusModel();

                    return new JobOrderModel
                    {
                        IsChangan = ul.IsChangan,
                        IsAllowedToOverride = isAllowedToOverride,
                        StatusChipColor = statusColor,
                        Id = ul.Id,
                        ReferenceNo = ul.ReferenceNo,
                        Customer = customerMap,
                        Vehicle = new VehiclesModel
                        {
                            Id = ul.Vehicle?.Id ?? 0,
                            VehicleModel = vehicleModelMap,
                            PlateNo = ul.Vehicle?.PlateNo,
                            YearModel = (int)ul.Vehicle?.YearModel
                        },
                        TransactionDate = ul.TransactionDate,
                        JobStatus = jobStatusMap
                    };
                }).ToList();

                // Replace list atomically to avoid multiple redraws
                JobOrderRequestModel = projected;

                IsLoading = false;
                StateHasChanged();
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
            data = JobOrderRequestModel.OrderBy(x => x.TransactionDate);

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

            if (!string.IsNullOrEmpty(clientTypeFilter))
            {
                if ($"{Constants.ClientType.Rapide}_{Constants.ClientType.Changan}".Contains(clientTypeFilter))
                {
                    bool clientType = clientTypeFilter.Equals(Constants.ClientType.Changan.ToString());

                    data = data.Where(x => x.IsChangan == clientType);
                }
            }

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

        private Task OnFilter(string text)
        {
            clientType = text;
            clientTypeFilter = text;
            return dataGrid.ReloadServerData();
        }
        #endregion

    }
}
