using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.Web.Helpers;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.Administrator
{
    public partial class UserRoleList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [Inject]
        protected NavigationManager? NavigationManager { get; set; }
        [Inject]
        private IRoleService? RoleService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private MudDataGrid<RoleModel>? dataGrid;
        private string? searchString;
        private List<RoleModel> RoleRequestModel = new List<RoleModel>();

        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            
            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task ReloadRequestModel()
        {
            try
            {
                var dataList = await RoleService.GetAllAsync();

                if (dataList == null)
                {
                    IsLoading = false;
                    return;
                }

                foreach (var ul in dataList)
                {
                    RoleRequestModel.Add(new RoleModel()
                    {
                        Id = ul.Id,
                        Name = ul.Name,
                        Description = ul.Description,
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

        private async Task<GridData<RoleModel>> ServerReload(GridState<RoleModel> state)
        {
            if (!RoleRequestModel.Any())
                await ReloadRequestModel();

            IEnumerable<RoleModel> data = new List<RoleModel>();
            data = RoleRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (!string.IsNullOrEmpty(element.Description))
                {
                    if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                
                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(RoleModel.Name):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Name
                        );
                        break;
                    case nameof(RoleModel.Description):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Description
                        );
                        break;
                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<RoleModel>
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
            NavigationManager.NavigateToCustom("/administrators/user-roles/add");
        }

        private async Task OnDeleteClick(RoleModel role)
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

                        await RoleService.DeleteAsync(role.Id);
                        SnackbarService.Add("User Role Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/administrators/user-roles", true);
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