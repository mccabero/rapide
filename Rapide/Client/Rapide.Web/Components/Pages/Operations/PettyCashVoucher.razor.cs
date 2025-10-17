using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using static MudBlazor.CategoryTypes;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class PettyCashVoucher
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
            IsLoading = true;
            isBigThreeRoles = TokenHelper.IsBigThreeRoles(await AuthState);

            #region Load Table
            // Get the last transaction to get the current balance.
            var pettyCashList = await PettyCashService.GetAllPettyCashAsync();

            LastPettyCashDto = pettyCashList == null
                ? null
                : pettyCashList.OrderByDescending(x => x.Id).FirstOrDefault();

            PettyCashModel.PCNo = await ReferenceNumberHelper.GetRNPettyCash(PettyCashService);
            PettyCashModel.TransactionDateTime = DateTime.Now;
            StateHasChanged();

            PettyCashModel.Balance = LastPettyCashDto == null
                ? 0
                : LastPettyCashDto.Balance;

            PettyCashModel.TransactionDateTime = LastPettyCashDto == null
                ? DateTime.Now
                : LastPettyCashDto.TransactionDateTime;
            StateHasChanged();

            #endregion

            // If route parameter is the literal "add", treat as create mode
            var isAddRoute = !string.IsNullOrEmpty(PettyCashVoucherId) &&
                             PettyCashVoucherId.Equals("add", StringComparison.OrdinalIgnoreCase);

            IsEditMode = !isAddRoute && !string.IsNullOrEmpty(PettyCashVoucherId);

            if (form != null)
                form.Disabled = true;

            // normalize PettyCashVoucherId for create route
            if (isAddRoute)
            {
                PettyCashVoucherId = null;
                form.Disabled = false;
            }

            if (IsEditMode)
            {
                if (int.TryParse(PettyCashVoucherId, out int pettyCashId))
                {
                    var pettyCashDto = await PettyCashService.GetPettyCashByIdAsync(pettyCashId);
                    if (pettyCashDto != null)
                    {
                        IMapper mapper = InitializeMapper();
                        PettyCashModel = mapper.Map<PettyCashModel>(pettyCashDto);
                        isChangan = PettyCashModel.IsChangan;
                        isCashIn = PettyCashModel.CashIn > 0;
                        Amount = isCashIn ? PettyCashModel.CashIn : PettyCashModel.CashOut;

                        form.Disabled = false;
                        StateHasChanged();
                    }
                    else
                    {
                        // If the record is not found, navigate back to list with error message.
                        SnackbarService.Add("Petty Cash Voucher not found.", Severity.Error, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/petty-cash-vouchers", true);
                    }
                }
                else
                {
                    // If the id is not valid, navigate back to list with error message.
                    SnackbarService.Add("Invalid Petty Cash Voucher ID.", Severity.Error, config => { config.ShowCloseIcon = true; });
                    NavigationManager.NavigateToCustom("/operations/petty-cash-vouchers", true);
                }
            }
                
            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        // Button handlers
        private async Task OnPettyCashNewClick()
        {
            mBoxCustomMessage = "Are you sure you want to add new record?";

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

            if (PettyCashModel.Balance <= 0)
            {
                mBoxCustomMessage = "Invalid input. The transaction will result to negative or zero (0) balance.";
                await mboxError.ShowAsync();
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

                if (IsEditMode)
                {
                    var pettyCashDto = await PettyCashService.GetPettyCashByIdAsync(PettyCashModel.Id);
                    pettyCashDTO.CreatedById = pettyCashDto.CreatedById;
                    pettyCashDTO.CreatedDateTime = pettyCashDto.CreatedDateTime;

                    pettyCashDTO.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                    pettyCashDTO.UpdatedDateTime = DateTime.Now;

                    await PettyCashService.UpdateAsync(pettyCashDTO);

                    // re-process records after update to adjust balances
                    var pettyCashList = await PettyCashService.GetAllPettyCashAsync();
                    var pettyCashListAfterDelete = pettyCashList
                        .Where(x => x.Id > pettyCashDTO.Id)
                        .OrderBy(x => x.Id)
                        .ToList();

                    foreach (var pc in pettyCashListAfterDelete)
                    {
                        var lastPc = pettyCashList
                            .Where(x => x.Id < pc.Id)
                            .OrderByDescending(x => x.Id).FirstOrDefault();

                        if (pc.CashIn > 0)
                            pc.Balance = (lastPc == null ? 0 : lastPc.Balance) + pc.CashIn;
                        else
                            pc.Balance = (lastPc == null ? 0 : lastPc.Balance) - pc.CashOut;
                       
                        await PettyCashService.UpdateAsync(pc);
                    }
                }
                else
                {
                    pettyCashDTO.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                    pettyCashDTO.CreatedDateTime = DateTime.Now;
                    pettyCashDTO.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                    pettyCashDTO.UpdatedDateTime = DateTime.Now;

                    await PettyCashService.CreateAsync(pettyCashDTO);
                }


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

            bool? result = await mbox.ShowAsync();
            var proceedCancel = result == null ? false : true;

            if (proceedCancel)
            {
                // Optionally set status or perform cleanup here.
                NavigationManager.NavigateToCustom("/operations/petty-cash-vouchers", true);
            }
        }

        private void OnAmountValueChanged(PettyCashModel model, decimal i)
        {
            PettyCashModel.Balance = 0;
            var lastPc = PettyCashModels
                            .Where(x => x.Id < model.Id)
                            .OrderByDescending(x => x.Id).FirstOrDefault();

            if (isCashIn)
            { 
                // Add to balance
                PettyCashModel.Balance = (lastPc == null ? 0 : lastPc.Balance) + i;
                PettyCashModel.CashIn = i;
                PettyCashModel.CashOut = 0;
            }
            else
            {
                // Deduct from balance
                PettyCashModel.Balance = (lastPc == null ? 0 : lastPc.Balance) - i;
                PettyCashModel.CashOut = i;
                PettyCashModel.CashIn = 0;
            }

            Amount = i;
            StateHasChanged();
        }

        // Action handlers called from the grid template
        private async Task OnPettyCashEdit(PettyCashModel model)
        {
            if (model == null)
                return;

            //// Prevent edit if the selected record is not the latest transaction
            //if (LastPettyCashDto != null && LastPettyCashDto.Id != model.Id)
            //{
            //    mBoxCustomMessage = "Updating old petty cash voucher is not allowed to prevent incorrect calculation of current balance." +
            //        "Please delete the latest record/s to enable editing this record.";
            //    await mboxError.ShowAsync();
            //    return;
            //}

            // Navigate to same page with record id for editing
            NavigationManager.NavigateToCustom($"/operations/petty-cash-vouchers/{model.Id}", true);
        }

        private async Task OnPettyCashDelete(PettyCashModel model)
        {
            try
            {
                if (model != null)
                {
                    // Prevent deletion if the selected record is not the latest transaction
                    if (LastPettyCashDto != null && LastPettyCashDto.Id != model.Id)
                    {
                        mBoxCustomMessage = "Deleting old petty cash voucher is not allowed to prevent incorrect calculation of current balance.";
                        await mboxError.ShowAsync();
                        return;
                    }

                    bool? result = await mbox.ShowAsync();
                    var proceed = result == null ? false : true;

                    if (proceed)
                    {
                        IsLoading = true;

                        await PettyCashService.DeleteAsync(model.Id);
                        SnackbarService.Add("Petty Cash Successfully Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/operations/petty-cash-vouchers", true);
                    }
                }
            }
            catch (Exception)
            {
                mBoxCustomMessage = "Unable to delete this record. This might be used in another transaction.";
                await mboxError.ShowAsync();

                IsLoading = false;
                StateHasChanged();
                return;
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

        private async Task ReloadRequestModel()
        {
            try
            {
                var dataList = await PettyCashService.GetAllPettyCashAsync();

                if (dataList == null)
                {
                    IsLoading = false;
                    return;
                }

                IMapper mapper = InitializeMapper();
                PettyCashModels = mapper.Map<List<PettyCashModel>>(dataList);
            }
            catch (Exception ex)
            {
                IsLoading = false;
                StateHasChanged();

                throw new Exception(ex.Message);
            }

        }

        private async Task<GridData<PettyCashModel>> ServerReload(GridState<PettyCashModel> state)
        {
            if (!PettyCashModels.Any())
                await ReloadRequestModel();

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
                cfg.CreateMap<UserModel, UserDTO>();
                cfg.CreateMap<RoleModel, RoleDTO>();

                cfg.CreateMap<PettyCashDTO, PettyCashModel>();
                cfg.CreateMap<UserDTO, UserModel>();
                cfg.CreateMap<RoleDTO, RoleModel>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }
    }
}