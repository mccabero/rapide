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
    public partial class ParameterList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IParameterService? ParameterService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private MudDataGrid<ParameterModel> dataGrid;
        private string searchString;
        private List<ParameterModel> ParameterRequestModel = new List<ParameterModel>();

        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }

        private bool isBigThreeRoles = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            isBigThreeRoles = TokenHelper.IsBigThreeRolesWithoutSupervisor(await AuthState);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var parameterList = await ParameterService.GetAllParameterAsync();

            if (parameterList == null)
                return;

            foreach (var pl in parameterList)
            {
                ParameterRequestModel.Add(new ParameterModel()
                {
                    Id = pl.Id,
                    Code = pl.Code,  
                    Name = pl.Name,
                    Description = pl.Description,
                    NumericData = pl.NumericData,
                    SortOrder = pl.SortOrder,
                    OtherData = pl.OtherData,
                    ParameterGroup = pl?.ParameterGroup?.Map<ParameterGroupModel>()
                });
            }

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task<GridData<ParameterModel>> ServerReload(GridState<ParameterModel> state)
        {
            IEnumerable<ParameterModel> data = new List<ParameterModel>();
            data = ParameterRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Id.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.ParameterGroup.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(ParameterModel.Code):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Code
                        );
                        break;
                    case nameof(ParameterModel.Name):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Name
                        );
                        break;
                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<ParameterModel>
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
            NavigationManager.NavigateToCustom("/configurations/parameters/add");
        }

        private async Task OnDeleteClick(ParameterModel parameterModel)
        {
            try
            {
                if (parameterModel != null)
                {
                    bool? result = await mbox.ShowAsync();
                    var proceed = result == null ? false : true;

                    if (proceed)
                    {
                        IsLoading = true;

                        await ParameterService.DeleteAsync(parameterModel.Id);
                        SnackbarService.Add("User Role Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/configurations/parameters", true);
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