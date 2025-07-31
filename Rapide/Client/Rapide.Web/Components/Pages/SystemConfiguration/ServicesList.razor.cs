using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class ServicesList
    {
        #region Parameters

        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IServiceService? ServiceService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private MudDataGrid<ServiceModel> dataGrid;
        private string searchString;
        private List<ServiceModel> ServiceRequestModel = new List<ServiceModel>();

        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }

        private bool isBigThreeRoles = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isBigThreeRoles = TokenHelper.IsBigThreeRolesWithoutSupervisor(await AuthState);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var dataList = await ServiceService.GetAllServiceAsync();

            if (dataList == null)
            {
                IsLoading = false;
                return;
            }

            foreach (var ul in dataList)
            {
                ServiceRequestModel.Add(new ServiceModel()
                {
                    Id = ul.Id,
                    Name = ul.Name,
                    Code = ul.Code,
                    ServiceGroup = ul.ServiceGroup.Map<ServiceGroupModel>(),
                    ServiceCategory = ul.ServiceCategory.Map<ServiceCategoryModel>(),
                    StandardRate = ul.StandardRate,
                    StandardHours = ul.StandardHours,
                    IsReplacement = ul.IsReplacement,
                    IsAllowRateOverride = ul.IsAllowRateOverride,
                    IsMechanicRequired = ul.IsMechanicRequired,
                    DisplayStandardHours = ul.DisplayStandardHours,
                    DisplayStandardRate = ul.DisplayStandardRate,
                    DisplayNotes = ul.DisplayNotes
                });
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task<GridData<ServiceModel>> ServerReload(GridState<ServiceModel> state)
        {
            IEnumerable<ServiceModel> data = new List<ServiceModel>();
            data = ServiceRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.StandardRate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.StandardHours.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.ServiceGroup.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.ServiceCategory.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(ServiceModel.Name):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Name
                        );
                        break;
                    case nameof(ServiceModel.Code):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Code
                        );
                        break;
                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<ServiceModel>
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
            NavigationManager.NavigateToCustom("/configurations/services/add");
        }

        private async Task OnDeleteClick(ServiceModel serviceModel)
        {
            try
            {
                if (serviceModel != null)
                {
                    bool? result = await mbox.ShowAsync();
                    var proceed = result == null ? false : true;

                    if (proceed)
                    {
                        IsLoading = true;

                        await ServiceService.DeleteAsync(serviceModel.Id);
                        SnackbarService.Add("Service Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/configurations/services", true);
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
    }
}