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
    public partial class PackagesList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IPackageService PackageService { get; set; }
        [Inject]
        private IPackageServiceService PackageServiceService { get; set; }
        [Inject]
        private IPackageProductService PackageProductService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }

        [Inject]
        private IEstimatePackageService EstimatePackageService { get; set; }
        [Inject]
        private IJobOrderPackageService JobOrderPackageService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; } = new();
        private MudMessageBox mboxError { get; set; } = new();
        private string mBoxCustomMessage { get; set; } = string.Empty;

        private bool IsLoading { get; set; }

        private MudDataGrid<PackageModel> dataGrid;
        private string searchString;
        private List<PackageModel> PackageRequestModel = new List<PackageModel>();

        private bool isBigThreeRoles = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            mBoxCustomMessage = string.Empty;
            IsLoading = true;
            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var dataList = await PackageService.GetAllPackageAsync();

            if (dataList == null)
            {
                IsLoading = false;
                return;
            }

            foreach (var ul in dataList)
            {
                PackageRequestModel.Add(new PackageModel()
                {
                    Id = ul.Id,
                    Name = ul.Name,
                    Code = ul.Code,
                    TotalAmount = ul.TotalAmount,
                });
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task<GridData<PackageModel>> ServerReload(GridState<PackageModel> state)
        {
            IEnumerable<PackageModel> data = new List<PackageModel>();
            data = PackageRequestModel.OrderBy(x => x.Code);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Id.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
               
                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(PackageModel.Id):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(PackageModel.Name):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Name
                        );
                        break;
                    case nameof(PackageModel.Code):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Code
                        );
                        break;
                   
                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<PackageModel>
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
            NavigationManager.NavigateToCustom("/configurations/packages/add");
        }

        private async Task OnDeleteClick(PackageModel model)
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

                        var estimatePackageList = await EstimatePackageService.GetAllEstimatePackageAsync();
                        var jobOrderPackageList = await JobOrderPackageService.GetAllJobOrderPackageAsync();

                        var estimatePackageById = estimatePackageList.Where(x => x.PackageId == model.Id);
                        var jobOrderPackageById = jobOrderPackageList.Where(x => x.PackageId == model.Id);

                        if (estimatePackageById.Any() || jobOrderPackageById.Any())
                        {
                            mBoxCustomMessage = "Unable to delete the this record. This might be used in another transaction.";
                            await mboxError.ShowAsync();

                            return;
                        }

                        // Delete Services
                        var packageServiceLIst = await PackageServiceService.GetAllPackageServiceByPackageIdAsync(model.Id);
                        if (packageServiceLIst != null)
                        {
                            foreach (var s in packageServiceLIst)
                                await PackageServiceService.DeleteAsync(s.Id);
                        }

                        // Delete Products
                        var packageProductList = await PackageProductService.GetAllPackageProductByPackageIdAsync(model.Id);
                        if (packageProductList != null)
                        {
                            foreach (var p in packageProductList)
                                await PackageProductService.DeleteAsync(p.Id);
                        }

                        await PackageService.DeleteAsync(model.Id);
                        SnackbarService.Add("Package Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/configurations/packages", true);
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