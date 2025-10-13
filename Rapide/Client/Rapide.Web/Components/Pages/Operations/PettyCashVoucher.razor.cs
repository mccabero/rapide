using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class PettyCashVoucher : ComponentBase
    {
        #region Parameters
        [Parameter]
        public string? PettyCashVoucherId { get; set; }
        #endregion

        #region Dependency Injection
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private IJobStatusService JobStatusService { get; set; }
        [Inject]
        private ICustomerService CustomerService { get; set; }
        [Inject]
        private IParameterService ParameterService { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IQuickSalesService QuickSalesService { get; set; }
        [Inject]
        private IQuickSalesProductService QuickSalesProductService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        #endregion

        #region Private Properties
        private MudForm form;
        private string[] errors = { };
        private bool success;

        private MudMessageBox mboxCustom { get; set; }
        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }
        private bool IsEditMode { get; set; }

        private PettyCashModel PettyCashModel { get; set; } = new();

        // Petty cash form fields
        private string PayTo { get; set; } = string.Empty;
        private decimal Amount { get; set; } = 0;
        private string Particulars { get; set; } = string.Empty;

        private List<JobStatusDTO> JobStatusList { get; set; } = new();
        private List<CustomerDTO> CustomerList { get; set; } = new();
        private List<ParameterDTO> PaymentTypeList { get; set; } = new();
        private List<UserDTO> SalesPersonUserList { get; set; } = new();

        private string JobStatusName = string.Empty;
        private string CustomerName = string.Empty;
        private bool isPettyCashLocked = false;
        private bool isBigThreeRoles = false;
        private bool isViewOnly = false;
        private bool isChangan = false;

        private bool isCashIn = false;

        // From child components
        public List<PettyCashModel> PettyCashModels { get; set; } = new();

        private MudDataGrid<PettyCashModel> dataGrid;
        private string searchString;

        // 1: Rapide | 2: Changan | 3: ALL
        public string clientType { get; set; } = Constants.ClientType.All;
        private string clientTypeFilter;
        #endregion

        // Button handlers
        private async Task OnPettyCashNewClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the current transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
                NavigationManager.NavigateToCustom("/operations/petty-cash-vouchers/add", true);
        }

        private async Task OnSavePettyCashClick()
        {
            if (form != null)
            {
                await form.Validate();
                if (!form.IsValid)
                    return;
            }

            bool? result = await mbox.ShowAsync();
            var proceedSaving = result == null ? false : true;

            if (!proceedSaving)
                return;

            try
            {
                IsLoading = true;

                // TODO: Implement actual save logic for Petty Cash Voucher here.
                // Placeholder - show success message and navigate back to list.
                SnackbarService.Add("Petty Cash Voucher successfully saved!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                NavigationManager.NavigateToCustom("/operations/petty-cash-vouchers", true);
            }
            catch (System.Exception ex)
            {
                SnackbarService.Add($"Error occurred while saving. {ex.Message}", Severity.Error, config => { config.ShowCloseIcon = true; });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OnCancelPettyCashClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the this transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedCancel = result == null ? false : true;

            if (proceedCancel)
            {
                // Optionally set status or perform cleanup here.
                NavigationManager.NavigateToCustom("/operations/petty-cash-vouchers", true);
            }
        }

        private Task OnFilter(string text)
        {
            clientType = text;
            clientTypeFilter = text;
            return dataGrid.ReloadServerData();
        }

        private Task OnSearch(string text)
        {
            searchString = text;
            return dataGrid.ReloadServerData();
        }

        private async Task<GridData<PettyCashModel>> ServerReload(GridState<PettyCashModel> state)
        {
            //if (!PettyCashModel.Any())
            //    await ReloadRequestModel();

            IEnumerable<PettyCashModel> data = new List<PettyCashModel>();
            data = PettyCashModels.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.PayTo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.JobStatus.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).ToArray();

            if (!string.IsNullOrEmpty(clientTypeFilter))
            {
                if ($"{Constants.ClientType.Rapide}_{Constants.ClientType.Changan}".Contains(clientTypeFilter))
                {
                    bool clientType = clientTypeFilter.Equals(Constants.ClientType.Changan.ToString());

                    data = data.Where(x => x.IsChangan == clientType);
                }
            }

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(PettyCashModel.Id):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(PettyCashModel.PayTo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.PayTo
                        );
                        break;

                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<PettyCashModel>
            {
                TotalItems = totalItems,
                Items = pagedData
            };
        }
    }
}
