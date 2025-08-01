using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.Vehicles
{
    public partial class VehicleList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IVehicleService? VehicleService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private MudDataGrid<VehiclesModel> dataGrid;
        private string searchString;
        private List<VehiclesModel> VehicleRequestModel = new List<VehiclesModel>();

        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }
        private bool isViewOnly = false;
        private bool IsBigThreeRoles = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            IsBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task ReloadRequestModel()
        {
            try
            {
                var dataList = await VehicleService.GetAllVehicleAsync();

                if (dataList == null)
                {
                    IsLoading = false;
                    return;
                }

                IMapper mapper = MappingWebHelper.InitializeMapper();
                //VehicleRequestModel = mapper.Map<List<VehiclesModel>>(dataList.ToList());

                foreach (var ul in dataList)
                {
                    var customerMap = mapper.Map<CustomerModel>(ul.Customer);
                    var vehicleModelMap = mapper.Map<VehicleModelModel>(ul.VehicleModel);

                    VehicleRequestModel.Add(new VehiclesModel()
                    {
                        Id = ul.Id,
                        Customer = customerMap,
                        FullName = $"{ul.Customer.FirstName} {ul.Customer.LastName}",
                        PlateNo = ul.PlateNo,
                        VehicleModel = vehicleModelMap,
                        VIN = ul.VIN,
                        YearModel = ul.YearModel
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

        private async Task<GridData<VehiclesModel>> ServerReload(GridState<VehiclesModel> state)
        {
            if (!VehicleRequestModel.Any())
                await ReloadRequestModel();

            IEnumerable<VehiclesModel> data = new List<VehiclesModel>();
            data = VehicleRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.PlateNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.VehicleModel.VehicleMake.Name} {element.VehicleModel.Name}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.YearModel.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Customer.FirstName} {element.Customer.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Customer.MobileNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase))
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
                            o => o.VehicleModel.Name
                        );
                        break;
                    case nameof(VehiclesModel.YearModel):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.YearModel
                        );
                        break;
                    case nameof(VehiclesModel.FullName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Customer.FirstName
                        );
                        break;
                    case $"Customer.{nameof(VehiclesModel.Customer.MobileNumber)}":
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Customer.MobileNumber
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

        private Task OnSearch(string text)
        {
            searchString = text;
            return dataGrid.ReloadServerData();
        }

        private void OnAddClick()
        {
            NavigationManager.NavigateToCustom("/vehicles/add");
        }

        private async Task OnDeleteClick(VehiclesModel model)
        {
            try
            {
                if (model != null)
                {
                    bool? result = await mbox.ShowAsync();
                    var proceed = result == null ? false : true;

                    if (proceed)
                    {
                        IsLoading = true;

                        await VehicleService.DeleteAsync(model.Id);
                        SnackbarService.Add("Vehicle Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/vehicles", true);
                    }
                }
            }
            catch (Exception)
            {
                mBoxCustomMessage = "Unable to delete the vehicle record. It's either the vehicle is already associated to a job order or has previous transaction.";
                await mboxError.ShowAsync();

                IsLoading = false;
                StateHasChanged();
                return;
            }
        }
    }
}