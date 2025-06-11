using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.PdfReportGenerator;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class ExpensesList
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
        private IExpensesService ExpensesService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private MudMessageBox mboxError { get; set; }
        private string mBoxCustomMessage { get; set; }
        private bool IsLoading { get; set; }
        private MudDataGrid<ExpensesModel> dataGrid;
        private string searchString;

        private List<ExpensesModel> ExpensesRequestModel = new List<ExpensesModel>();
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var dataList = await ExpensesService.GetAllExpensesAsync();

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

                ExpensesRequestModel.Add(new ExpensesModel()
                {
                    IsAllowedToOverride = TokenHelper.IsBigThreeRoles(await AuthState),
                    StatusChipColor = statusColor,
                    Id = ul.Id,
                    ReferenceNo = ul.ReferenceNo,
                    ExpenseDateTime = ul.ExpenseDateTime,
                    Amount = ul.Amount,
                    VAT12 = ul.VAT12,
                    PayTo = ul.PayTo,
                    Remarks = ul.Remarks,
                    PaymentReferenceNo = ul.PaymentReferenceNo,
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
                    ExpenseByUser = new UserModel()
                    {
                        Id = ul.ExpenseByUser.Id,
                        FirstName = ul.ExpenseByUser.FirstName,
                        LastName = ul.ExpenseByUser.LastName
                    }
                });
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task<GridData<ExpensesModel>> ServerReload(GridState<ExpensesModel> state)
        {
            IEnumerable<ExpensesModel> data = new List<ExpensesModel>();
            data = ExpensesRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.ReferenceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.ExpenseByUser.FirstName} {element.ExpenseByUser.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.ExpenseDateTime.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Amount.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(ExpensesModel.ReferenceNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(ExpensesModel.ExpenseByUser):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.ExpenseByUser.FirstName
                        );
                        break;
                    case nameof(ExpensesModel.ExpenseDateTime):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.ExpenseDateTime
                        );
                        break;
                    case nameof(ExpensesModel.Amount):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Amount
                        );
                        break;

                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<ExpensesModel>
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
            NavigationManager.NavigateToCustom("/operations/expenses/add");
        }

        private async Task OnDeleteClick(ExpensesModel model)
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

                        await ExpensesService.DeleteAsync(model.Id);
                        SnackbarService.Add("Expenses Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/operations/expenses", true);
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