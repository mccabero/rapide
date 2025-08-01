using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class MembershipList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IMembershipService? MembershipService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private MudDataGrid<MembershipModel> dataGrid;
        private string searchString;
        private List<MembershipModel> MembershipRequestModel = new List<MembershipModel>();

        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }

        private bool isBigThreeRoles = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);
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
                var datalist = await MembershipService.GetAllMembershipAsync();

                if (datalist == null)
                {
                    IsLoading = false;
                    return;
                }

                foreach (var ul in datalist)
                {
                    MembershipRequestModel.Add(new MembershipModel()
                    {
                        Customer = ul.Customer.Map<CustomerModel>(),
                        Id = ul.Id,
                        MembershipNo = ul.MembershipNo,
                        MembershipDate = ul.MembershipDate,
                        ExpiryDate = ul.ExpiryDate,
                        IsActive = ul.IsActive,
                        CreatedDateTime = ul.CreatedDateTime,
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

        private async Task<GridData<MembershipModel>> ServerReload(GridState<MembershipModel> state)
        {
            if (!MembershipRequestModel.Any())
                await ReloadRequestModel();

            IEnumerable<MembershipModel> data = new List<MembershipModel>();
            data = MembershipRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Id.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.MembershipNo.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Customer.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Customer.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Customer.MiddleName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Customer.MobileNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Customer.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(MembershipModel.Id):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(MembershipModel.Customer.FirstName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(MembershipModel.Customer.MiddleName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(MembershipModel.Customer.LastName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(MembershipModel.Customer.MobileNumber):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(MembershipModel.Customer.Email):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<MembershipModel>
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
            NavigationManager.NavigateToCustom("/configurations/memberships/add");
        }

        private async Task OnDeleteClick(MembershipModel uom)
        {
            try
            {
                if (uom != null)
                {
                    bool? result = await mbox.ShowAsync();
                    var proceed = result == null ? false : true;

                    if (proceed)
                    {
                        IsLoading = true;

                        await MembershipService.DeleteAsync(uom.Id);
                        SnackbarService.Add("Membership Successfully Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/configurations/memberships", true);
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