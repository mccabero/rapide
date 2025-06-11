using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.Services;
using Rapide.Web.Helpers;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.Administrator
{
    public partial class UserList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IUserRolesService UserRolesService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }

        private MudDataGrid<UserModel> dataGrid;
        private string searchString = null;
        private List<UserModel> users = new List<UserModel>();

        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            var dataList = await UserService.GetAllUserRoleAsync();

            if (dataList == null)
            {
                IsLoading = false;
                return;
            }

            foreach (var ul in dataList)
            {
                users.Add(new UserModel()
                {
                    Id = ul.Id,
                    FirstName = ul.FirstName,
                    MiddleName = ul.MiddleName,
                    LastName = ul.LastName,
                    Email = ul.Email,
                    MobileNumber = ul.MobileNumber,
                    FullName = $"{ul.FirstName} {ul.MiddleName} {ul.LastName}",
                    Gender = (int)ul.Gender,
                    IsActive = ul.IsActive,
                    RoleId = (int)ul.RoleId,
                    Role = ul?.Role?.Map<RoleModel>()
                });
            }

            IsLoading = false;
            StateHasChanged();
            base.OnInitializedAsync();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        private async Task<GridData<UserModel>> ServerReload(GridState<UserModel> state)
        {
            IEnumerable<UserModel> data = new List<UserModel>();
            data = users.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.FirstName} {element.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Role.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
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
                    case nameof(UserModel.FirstName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.FirstName
                        );
                        break;
                    case nameof(UserModel.FullName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.FullName
                        );
                        break;
                    case nameof(UserModel.Email):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Email
                        );
                        break;
                    case nameof(UserModel.MobileNumber):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.MobileNumber
                        );
                        break;
                    case nameof(UserModel.Role.Name):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Role.Name
                        );
                        break;
                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new GridData<UserModel>
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
            NavigationManager.NavigateToCustom("/administrators/users/add");
        }

        private async Task OnDeleteClick(UserModel user)
        {
            try
            {
                if (user != null)
                {
                    bool? result = await mbox.ShowAsync();
                    var proceed = result == null ? false : true;

                    if (proceed)
                    {
                        IsLoading = true;

                        // Delete Products
                        var userRolesList = await UserRolesService.GetUserRolesByUserIdAsync(user.Id);
                        if (userRolesList != null)
                        {
                            foreach (var p in userRolesList)
                                await UserRolesService.DeleteAsync(p.Id);
                        }

                        await UserService.DeleteAsync(user.Id);
                        SnackbarService.Add("User Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/administrators/users", true);
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
