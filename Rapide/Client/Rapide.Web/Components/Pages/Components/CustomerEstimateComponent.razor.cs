using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.Web.Helpers;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class CustomerEstimateComponent
    {
        #region Parameters
        [Parameter]
        public string? CustomerIdParam { get; set; }
        #endregion

        #region Dependency Injection
        [Inject]
        private IEstimateService? EstimateService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private List<EstimateModel> EstimateRequestModel = new List<EstimateModel>();
        private MudDataGrid<EstimateModel> dataGrid;
        private string searchString;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(CustomerIdParam))
                return;

            var customerId =  int.Parse(CustomerIdParam);

            var dataList = await EstimateService.GetAllEstimateByCustomerIdAsync(customerId);

            if (dataList == null)
            {
                return;
            }

            foreach (var ul in dataList)
            {
                EstimateRequestModel.Add(new EstimateModel()
                {
                    Id = ul.Id,
                    Customer = ul.Customer.Map<CustomerModel>(),
                    ReferenceNo = ul.ReferenceNo,
                    Vehicle = new VehiclesModel()
                    {
                        Id = ul.Vehicle.Id,
                        PlateNo = ul.Vehicle.PlateNo,
                        VehicleModel = new VehicleModelModel()
                        {
                            Id = ul.Vehicle.VehicleModel.Id,
                            VehicleMake = new VehicleMakeModel()
                            {
                                Id = ul.Vehicle.VehicleModel.VehicleMake.Id,
                                Name = ul.Vehicle.VehicleModel.VehicleMake.Name,
                                Description = ul.Vehicle.VehicleModel.VehicleMake.Description
                            },
                            Name = ul.Vehicle.VehicleModel.Name,
                            BodyParameter = new ParameterModel()
                            {
                                Id = ul.Vehicle.VehicleModel.BodyParameter.Id,
                                Name = ul.Vehicle.VehicleModel.BodyParameter.Name,
                                Description = ul.Vehicle.VehicleModel.BodyParameter.Description
                            },
                            ClassificationParameter = new ParameterModel()
                            {
                                Id = ul.Vehicle.VehicleModel.ClassificationParameter.Id,
                                Name = ul.Vehicle.VehicleModel.ClassificationParameter.Name,
                                Description = ul.Vehicle.VehicleModel.ClassificationParameter.Description
                            },
                            Description = ul.Vehicle.VehicleModel.Description
                        }
                    }
                });
            }
        }

        private async Task<GridData<EstimateModel>> ServerReload(GridState<EstimateModel> state)
        {
            IEnumerable<EstimateModel> data = new List<EstimateModel>();
            data = EstimateRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.ReferenceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Vehicle.VehicleModel.VehicleMake.Name} {element.Vehicle.VehicleModel.Name}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Customer.FirstName} {element.Customer.MiddleName} {element.Customer.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Vehicle.PlateNo.Contains(searchString, StringComparison.OrdinalIgnoreCase))
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
                            o => o.ReferenceNo
                        );
                        break;
                    case nameof(EstimateModel.Customer.FirstName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Customer.FirstName
                        );
                        break;
                    case nameof(EstimateModel.Vehicle.VehicleModel.VehicleMake.Name):
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

        private void OnAddNewClick()
        {
            NavigationManager.NavigateToCustom("/operations/estimates/add", true);
        }

        private Task OnSearch(string text)
        {
            searchString = text;
            return dataGrid.ReloadServerData();
        }
    }
}