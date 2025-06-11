
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.PdfReportGenerator;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class QuickSalesList
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
        private IQuickSalesService QuickSalesService { get; set; }
        [Inject]
        private IQuickSalesProductService QuickSalesProductService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private MudMessageBox mboxError { get; set; }
        private string mBoxCustomMessage { get; set; }
        private bool IsLoading { get; set; }
        private MudDataGrid<QuickSalesModel> dataGrid;
        private string searchString;

        private List<QuickSalesModel> QuickSalesRequestModel = new List<QuickSalesModel>();

        private bool isQuickSalesLocked = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var dataList = await QuickSalesService.GetAllQuickSalesAsync();

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

                QuickSalesRequestModel.Add(new QuickSalesModel()
                {
                    IsAllowedToOverride = TokenHelper.IsBigThreeRoles(await AuthState),
                    StatusChipColor = statusColor,
                    Id = ul.Id,
                    ReferenceNo = ul.ReferenceNo,
                    TransactionDate = ul.TransactionDate,
                    Customer = new CustomerModel()
                    { 
                        Id = ul.Customer.Id,
                        FirstName = ul.Customer.FirstName,
                        LastName = ul.Customer.LastName
                    },
                    JobStatus = new JobStatusModel()
                    { 
                        Id = ul.JobStatus.Id,
                        Name = ul.JobStatus.Name
                    },
                    PaymentTypeParameter = new ParameterModel()
                    { 
                        Id = ul.PaymentTypeParameter.Id,
                        Name = ul.PaymentTypeParameter.Name
                    },
                    PaymentReferenceNo = ul.PaymentReferenceNo,
                    SalesPersonUser = new UserModel()
                    { 
                        Id = ul.SalesPersonUser.Id,
                        FirstName = ul.SalesPersonUser.FirstName,
                        LastName= ul.SalesPersonUser.LastName
                    },
                    Summary = ul.Summary,
                    SubTotal = ul.SubTotal,
                    VAT12 = ul.VAT12,
                    Discount = ul.Discount,
                    TotalAmount = ul.TotalAmount,
                    Payment = ul.Payment,
                    Change = ul.Change
                });
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task<GridData<QuickSalesModel>> ServerReload(GridState<QuickSalesModel> state)
        {
            IEnumerable<QuickSalesModel> data = new List<QuickSalesModel>();
            data = QuickSalesRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.ReferenceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Customer.FirstName} {element.Customer.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.PaymentTypeParameter.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.TransactionDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.TotalAmount.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(QuickSalesModel.ReferenceNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.ReferenceNo
                        );
                        break;
                    case nameof(QuickSalesModel.Customer):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Customer.FirstName
                        );
                        break;
                    case nameof(QuickSalesModel.TransactionDate):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.TransactionDate
                        );
                        break;
                    case nameof(QuickSalesModel.TotalAmount):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.TotalAmount
                        );
                        break;

                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<QuickSalesModel>
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
            NavigationManager.NavigateToCustom("/operations/quick-sales/add");
        }

        private async Task OnDeleteClick(QuickSalesModel model)
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

                        // Delete Products
                        var quickSalesProductList = await QuickSalesProductService.GetAllQuickSalesProductByQuickSalesIdAsync(model.Id);
                        if (quickSalesProductList != null)
                        {
                            foreach (var p in quickSalesProductList)
                                await QuickSalesProductService.DeleteAsync(p.Id);
                        }

                        await QuickSalesService.DeleteAsync(model.Id);
                        SnackbarService.Add("Quick Sales Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/operations/quick-sales", true);
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

        private async Task OnGeneratePdfClick(int QuickSalesId)
        {
            var QuickSalesRequestModel = await QuickSalesService.GetQuickSalesByIdAsync(QuickSalesId);

            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            // if status is not completed, print is not available
            if (QuickSalesRequestModel != null) 
            {
                if (!QuickSalesRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Completed))
                {
                    mBoxCustomMessage = "Printing of quick sales is not available for OPEN status!";
                    await mboxError.ShowAsync();

                    return;
                }

                QuickSalesRequestModel.ProductList = await QuickSalesProductService.GetAllQuickSalesProductByQuickSalesIdAsync(QuickSalesId);

                if (QuickSalesRequestModel == null)
                    return;

                QuickSalesReportGenerator.ImageFile = FileHelper.GetRapideLogo();
                QuickSalesReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
                await QuickSalesReportGenerator.Generate(QuickSalesRequestModel, JSRuntime, companyData);
            }
        }
    }
}