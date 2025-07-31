using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.Customers
{
    public partial class CustomerList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        protected NavigationManager? NavigationManager { get; set; }
        [Inject]
        private ICustomerService? CustomerService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private MudDataGrid<CustomerModel>? dataGrid;
        private string? searchString;
        private List<CustomerModel> CustomerRequestModel = new List<CustomerModel>();

        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }
        private bool isViewOnly = false;
        private bool isBigThreeRoles = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isBigThreeRoles = TokenHelper.IsBigThreeRolesWithoutSupervisor(await AuthState);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            try
            {
                var dataList = await CustomerService.GetAllAsync();

                if (dataList == null)
                {
                    IsLoading = false;
                    return;
                }

                IMapper mapper = MappingWebHelper.InitializeMapper();
                CustomerRequestModel = mapper.Map<List<CustomerModel>>(dataList);
            }
            catch (Exception ex)
            {
                IsLoading = false;
                StateHasChanged();

                throw new Exception(ex.Message);
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task<GridData<CustomerModel>> ServerReload(GridState<CustomerModel> state)
        {
            IEnumerable<CustomerModel> data = new List<CustomerModel>();
            data = CustomerRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if ($"{element.FirstName} {element.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (!string.IsNullOrEmpty(element.HomeAddress))
                {
                    if (element.HomeAddress.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                if (element.MobileNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(CustomerModel.FirstName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.FirstName
                        );
                        break;
                    case nameof(CustomerModel.HomeAddress):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.HomeAddress
                        );
                        break;
                    case nameof(CustomerModel.MobileNumber):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.MobileNumber
                        );
                        break;
                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<CustomerModel>
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
            NavigationManager.NavigateToCustom("/customers/add");
        }

        private async Task OnDeleteClick(CustomerModel role)
        {
            try
            {
                if (role != null)
                {
                    bool? result = await mbox.ShowAsync();
                    var proceed = result == null ? false : true;

                    if (proceed)
                    {
                        IsLoading = true;

                        await CustomerService.DeleteAsync(role.Id);
                        SnackbarService.Add("Customer Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/customers", true);
                    }
                }
            }
            catch (Exception ex)
            {
                mBoxCustomMessage = "Unable to delete the customer record. It's either the customer is already associated to a job order or has previous transaction.";
                await mboxError.ShowAsync();

                IsLoading = false;
                StateHasChanged();
                return;
            }
        }
    }
}