using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.Web.Helpers;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class CustomerVehiclesComponent
    {
        #region Parameters
        [Parameter]
        public string? CustomerIdParam { get; set; }
        #endregion

        #region Dependency Injection
        [Inject]
        private IVehicleService? VehicleService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private List<VehiclesModel> VehicleRequestModel = new List<VehiclesModel>();
        private MudDataGrid<VehiclesModel> dataGrid;
        private string searchString;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(CustomerIdParam))
                return;

            var customerId =  int.Parse(CustomerIdParam);

            var dataList = await VehicleService.GetAllVehicleByCustomerIdAsync(customerId);

            if (dataList == null)
            {
                return;
            }

            foreach (var ul in dataList)
            {
                VehicleRequestModel.Add(new VehiclesModel()
                {
                    Id = ul.Id,
                    Customer = ul.Customer.Map<CustomerModel>(),
                    FullName = $"{ul.Customer.FirstName} {ul.Customer.LastName}",
                    PlateNo = ul.PlateNo,
                    VehicleModel = new VehicleModelModel()
                    {
                        Id = ul.VehicleModel.Id,
                        VehicleMake = new VehicleMakeModel()
                        {
                            Id = ul.VehicleModel.VehicleMake.Id,
                            Name = ul.VehicleModel.VehicleMake.Name,
                            Description = ul.VehicleModel.VehicleMake.Description
                        },
                        Name = ul.VehicleModel.Name,
                        BodyParameter = new ParameterModel()
                        {
                            Id = ul.VehicleModel.BodyParameter.Id,
                            Name = ul.VehicleModel.BodyParameter.Name,
                            Description = ul.VehicleModel.BodyParameter.Description
                        },
                        ClassificationParameter = new ParameterModel()
                        {
                            Id = ul.VehicleModel.ClassificationParameter.Id,
                            Name = ul.VehicleModel.ClassificationParameter.Name,
                            Description = ul.VehicleModel.ClassificationParameter.Description
                        },
                        Description = ul.VehicleModel.Description
                    },
                    VIN = ul.VIN,
                    YearModel = ul.YearModel
                });
            }
        }

        private async Task<GridData<VehiclesModel>> ServerReload(GridState<VehiclesModel> state)
        {
            IEnumerable<VehiclesModel> data = new List<VehiclesModel>();
            data = VehicleRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Id.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.VehicleModel.VehicleMake.Name} {element.VehicleModel.Name}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Customer.FirstName} {element.Customer.MiddleName} {element.Customer.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.PlateNo.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(VehiclesModel.PlateNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.PlateNo
                        );
                        break;
                    case nameof(VehiclesModel.VehicleModel.VehicleMake.Name):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.VehicleModel.VehicleMake.Name
                        );
                        break;
                    case nameof(VehiclesModel.YearModel):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.YearModel
                        );
                        break;
                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<VehiclesModel>
            {
                TotalItems = totalItems,
                Items = pagedData
            };
        }

        private void OnAddNewClick()
        {
            NavigationManager.NavigateToCustom("/customers/add", true);
        }

        private Task OnSearch(string text)
        {
            searchString = text;
            return dataGrid.ReloadServerData();
        }
    }
}