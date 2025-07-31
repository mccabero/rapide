using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.Entities;
using Rapide.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.PdfReportGenerator;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class DepositList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private IDepositService DepositService { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private MudMessageBox mboxError { get; set; }
        private string mBoxCustomMessage { get; set; }
        private bool IsLoading { get; set; }
        private MudDataGrid<DepositModel> dataGrid;
        private string searchString;

        private List<DepositModel> DepositRequestModel = new List<DepositModel>();
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var dataList = await DepositService.GetAllDepositAsync();

            if (dataList == null)
            {
                IsLoading = false;
                return;
            }

            foreach (var ul in dataList)
            {
                Color statusColor = Color.Primary;
                if (ul.JobStatus.Name.Equals(Constants.JobStatus.Open))
                    statusColor = Color.Warning;
                else if (ul.JobStatus.Name.Equals(Constants.JobStatus.Completed))
                    statusColor = Color.Success;
                else if (ul.JobStatus.Name.Equals(Constants.JobStatus.Cancelled))
                    statusColor = Color.Error;

                DepositRequestModel.Add(new DepositModel()
                {
                    IsAllowedToOverride = TokenHelper.IsBigThreeRolesWithoutSupervisor(await AuthState),
                    StatusChipColor = statusColor,
                    Id = ul.Id,
                    ReferenceNo = ul.ReferenceNo,
                    TransactionDateTime = ul.TransactionDateTime,
                    DepositAmount = ul.DepositAmount,
                    PaymentReferenceNo = ul.PaymentReferenceNo,
                    Description = ul.Description,
                    PaymentTypeParameter = new ParameterModel()
                    {
                        Id = ul.PaymentTypeParameter.Id,
                        Name = ul.PaymentTypeParameter.Name,
                    },
                    JobStatus = new JobStatusModel()
                    {
                        Id = ul.JobStatus.Id,
                        Name = ul.JobStatus.Name
                    },
                    JobOrder = new JobOrderModel()
                    {
                        Id = ul.JobOrder.Id,
                        ReferenceNo = ul.JobOrder.ReferenceNo
                    },
                    Customer = new CustomerModel()
                    {
                        Id = ul.Customer.Id,
                        FirstName = ul.Customer.FirstName,
                        LastName = ul.Customer.LastName
                    }
                });
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task<GridData<DepositModel>> ServerReload(GridState<DepositModel> state)
        {
            IEnumerable<DepositModel> data = new List<DepositModel>();
            data = DepositRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.ReferenceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Customer.FirstName} {element.Customer.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.TransactionDateTime.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.JobOrder.ReferenceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.DepositAmount.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(DepositModel.ReferenceNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(DepositModel.Customer):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Customer.FirstName
                        );
                        break;
                    case nameof(DepositModel.JobOrder):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.JobOrder.ReferenceNo
                        );
                        break;
                    case nameof(DepositModel.TransactionDateTime):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.TransactionDateTime
                        );
                        break;
                    case nameof(DepositModel.DepositAmount):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.DepositAmount
                        );
                        break;
                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<DepositModel>
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
            NavigationManager.NavigateToCustom("/operations/deposits/add");
        }

        private async Task OnDeleteClick(DepositModel model)
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

                        await DepositService.DeleteAsync(model.Id);
                        SnackbarService.Add("Deposit Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/operations/deposits", true);
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

        private async Task OnGeneratePdfClick(int depositId)
        {
            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            var depositRequestModel = await DepositService.GetDepositByIdAsync(depositId);

            if (depositRequestModel == null)
                return;

            depositRequestModel.PreparedBy = await UserService.GetUserRoleByIdAsync(depositRequestModel.UpdatedById);

            DepositReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            DepositReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
            await DepositReportGenerator.Generate(depositRequestModel, JSRuntime, companyData);
        }
    }
}