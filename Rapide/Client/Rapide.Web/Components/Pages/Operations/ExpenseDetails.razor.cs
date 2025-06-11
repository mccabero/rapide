using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class ExpenseDetails
    {
        #region Parameters
        [Parameter]
        public string? ExpensesId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IParameterService ParameterService { get; set; }
        [Inject]
        private IExpensesService ExpensesService { get; set; }
        [Inject]
        private IJobStatusService JobStatusService { get; set; }
        #endregion

        #region Private Properties
        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxCustom { get; set; }
        private MudMessageBox mboxError { get; set; }
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }
        private bool IsEditMode { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private ExpensesDTO ExpensesRequestModel { get; set; } = new();

        private List<UserDTO> ExpensesByUserList { get; set; } = new();
        private List<ParameterDTO> PaymentTypeList { get; set; } = new();
        private List<JobStatusDTO> JobStatusList { get; set; } = new();
        private bool isExpensesLocked = false;
        private bool isBigThreeRoles = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var userList = await UserService.GetAllUserRoleAsync();
            var paymentTypeList = await ParameterService.GetAllParameterAsync();
            JobStatusList = await JobStatusService.GetAllAsync();

            ExpensesByUserList = userList;
            PaymentTypeList = paymentTypeList.Where(x => x.ParameterGroup.Name.Equals(Constants.ParameterType.PaymentTypeParam)).ToList();

            IsEditMode = !string.IsNullOrEmpty(ExpensesId);
            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);

            if (IsEditMode)
            {
                await ReloadExpensesData();

                // creteria of locked based on status.
                isExpensesLocked =
                    ExpensesRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Converted) ||
                    ExpensesRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Completed) ||
                    ExpensesRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Cancelled) || 
                    isViewOnly;

                form.Disabled = isExpensesLocked;
                IsEditMode = !isExpensesLocked;
            }
            else
            {
                ExpensesRequestModel.JobStatus = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                ExpensesRequestModel.ExpenseDateTime = DateTime.Now;

                ExpensesRequestModel.ReferenceNo = await ReferenceNumberHelper.GetRNExpenses(ExpensesService);
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task OnSaveClick()
        {
            await form.Validate();

            if (!form.IsValid)
                return;

            bool? result = await mbox.ShowAsync();
            var proceedSaving = result == null ? false : true;

            if (proceedSaving)
            {
                try
                {
                    IsLoading = true;
                    bool isEditMode = !string.IsNullOrEmpty(ExpensesId);

                    ExpensesRequestModel.PaymentTypeParameterId = ExpensesRequestModel.PaymentTypeParameter.Id;
                    ExpensesRequestModel.ExpenseByUserId = ExpensesRequestModel.ExpenseByUser.Id;
                    ExpensesRequestModel.JobStatusId = ExpensesRequestModel.JobStatus.Id;

                    // Temporary set to zero
                    ExpensesRequestModel.VAT12 = 0;

                    if (!isEditMode) // create mode
                    {
                        ExpensesRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ExpensesRequestModel.CreatedDateTime = DateTime.Now;
                        ExpensesRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ExpensesRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        ExpensesRequestModel.JobStatus = ExpensesRequestModel.IsPaid
                            ? JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Completed)).FirstOrDefault()
                            : JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();

                        ExpensesRequestModel.JobStatusId = ExpensesRequestModel.JobStatus.Id;

                        var created = await ExpensesService.CreateAsync(ExpensesRequestModel);

                        ExpensesId = created.Id.ToString();
                        await ReloadExpensesData();

                        IsLoading = false;
                        SnackbarService.Add("Expenses Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/expenses", true);

                    }
                    else // update mode
                    {
                        int estimateId = int.Parse(ExpensesId);

                        ExpensesRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ExpensesRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        ExpensesRequestModel.JobStatus = ExpensesRequestModel.IsPaid
                            ? JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Completed)).FirstOrDefault()
                            : JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();

                        ExpensesRequestModel.JobStatusId = ExpensesRequestModel.JobStatus.Id;

                        await ExpensesService.UpdateAsync(ExpensesRequestModel);

                        await ReloadExpensesData();

                        SnackbarService.Add("Expenses Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/expenses", true);
                        IsLoading = false;

                    }
                }
                catch (Exception ex)
                {
                    SnackbarService.Add(
                        $"Error occurred while processing the transaction. Please contact your systems administrator.{Environment.NewLine}" +
                        $"Error Message: {ex.Message} ",
                        Severity.Error,
                        config => { config.ShowCloseIcon = true; });

                    IsLoading = false;
                }
            }
        }

        private async Task OnReOpenClick()
        {
            if (string.IsNullOrEmpty(ExpensesId))
                return;

            bool? result = await mbox.ShowAsync();
            var proceed = result == null ? false : true;

            if (proceed)
            {
                IsLoading = true;

                var jobStatusOpen = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                ExpensesRequestModel.JobStatus = jobStatusOpen;
                ExpensesRequestModel.JobStatusId = jobStatusOpen.Id;
                ExpensesRequestModel.IsPaid = false;

                await ExpensesService.UpdateAsync(ExpensesRequestModel);
                SnackbarService.Add("Expenses Successfuly re-OPENED!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                IsLoading = false;
                StateHasChanged();

                NavigationManager.NavigateToCustom("/operations/expenses", true);
            }
        }

        private async Task OnCancelClick()
        {
            NavigationManager.NavigateToCustom("/operations/expenses");
        }

        private async Task OnNewExpensesClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the current transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
                NavigationManager.NavigateToCustom("/operations/expenses/add", true);
        }

        private async Task OnCancelExpensesClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the this transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                var jobStatusCancelled = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Cancelled)).FirstOrDefault();
                ExpensesRequestModel.JobStatus = jobStatusCancelled;
                ExpensesRequestModel.JobStatusId = jobStatusCancelled.Id;

                await ExpensesService.UpdateAsync(ExpensesRequestModel);

                SnackbarService.Add("Expense Successfuly Cancelled!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                NavigationManager.NavigateToCustom("/operations/expenses", true);
            }
        }

        private async Task ReloadExpensesData()
        {
            // Get data by id
            ExpensesRequestModel = await ExpensesService.GetExpensesByIdAsync(int.Parse(ExpensesId));
        }

        private void OnAddNewItemClick(DialogTypes dt)
        {
            bool isEditMode = !string.IsNullOrEmpty(ExpensesId);
            var returnUrl = isEditMode
                ? $"/operations/expenses/{ExpensesId}"
                : "/operations/expenses/add";

            if (dt == DialogTypes.Payment)
                NavigationManager.NavigateToCustom($"/configurations/parameters/add?returnUrl={returnUrl}");
            if (dt == DialogTypes.User)
                NavigationManager.NavigateToCustom($"/administrators/users/add?returnUrl={returnUrl}");
        }

        private async Task<IEnumerable<UserDTO>> SearchExpensesByUser(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ExpensesByUserList;

            return ExpensesByUserList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
        private async Task<IEnumerable<ParameterDTO>> SearchPaymentType(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return PaymentTypeList;

            return PaymentTypeList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}