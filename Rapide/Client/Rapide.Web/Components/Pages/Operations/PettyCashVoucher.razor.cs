using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;

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
        private IPettyCashService PettyCashService { get; set; }
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
        private PettyCashDTO? LastPettyCashDto { get; set; }

        // Petty cash form fields
        private decimal Amount { get; set; } = 0;

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

        protected override async Task OnInitializedAsync()
        {
            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);

            // Get the last transaction to get the current balance.
            var pettyCashList = await PettyCashService.GetAllPettyCashAsync();

            LastPettyCashDto = pettyCashList == null
                ? new()
                : pettyCashList.OrderByDescending(x => x.Id).FirstOrDefault();

            PettyCashModel.PCNo = await ReferenceNumberHelper.GetRNPettyCash(PettyCashService);
            PettyCashModel.TransactionDateTime = DateTime.Now;

            PettyCashModel.Balance = LastPettyCashDto == null 
                ? 0 
                : LastPettyCashDto.Balance;
        }

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
                PettyCashModel.PaidByUserId = TokenHelper.GetCurrentUserId(authState: await AuthState);

                if (isCashIn)
                    PettyCashModel.CashIn = Amount;
                else
                    PettyCashModel.CashOut = Amount;


                IMapper mapper = InitializeMapper();
                var pettyCashDTO = mapper.Map<PettyCashDTO>(PettyCashModel);

                pettyCashDTO.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                pettyCashDTO.CreatedDateTime = DateTime.Now;
                pettyCashDTO.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                pettyCashDTO.UpdatedDateTime = DateTime.Now;

                await PettyCashService.CreateAsync(pettyCashDTO);

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

        private void OnAmountValueChanged(PettyCashModel model, decimal i)
        {
            if (isCashIn)
            { 
                // Add to balance
                PettyCashModel.Balance = (LastPettyCashDto == null ? i : LastPettyCashDto.Balance) + i;
                PettyCashModel.CashIn = i;
                PettyCashModel.CashOut = 0;
            }
            else
            {
                // Deduct from balance
                PettyCashModel.Balance = (LastPettyCashDto == null ? i : LastPettyCashDto.Balance) - i;
                PettyCashModel.CashOut = i;
                PettyCashModel.CashIn = 0;
            }

            Amount = i;
            StateHasChanged();
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

        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PettyCashModel, PettyCashDTO>();

                cfg.CreateMap<PettyCashDTO, PettyCashModel>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }
    }
}
