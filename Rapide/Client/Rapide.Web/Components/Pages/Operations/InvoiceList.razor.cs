using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.PdfReportGenerator;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class InvoiceList
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        private IInvoiceService InvoiceService { get; set; }
        [Inject]
        private IJobOrderService JobOrderService { get; set; }
        [Inject]
        private IJobOrderServiceService JobOrderServiceService { get; set; }
        [Inject]
        private IJobOrderProductService JobOrderProductService { get; set; }
        [Inject]
        private IJobOrderTechnicianService JobOrderTechnicianService { get; set; }
        [Inject]
        private IPackageService PackageService { get; set; }
        [Inject]
        private IInvoicePackageService InvoicePackageService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        [Inject]
        private IDepositService DepositService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private MudMessageBox mboxError { get; set; }
        private string mBoxCustomMessage { get; set; }
        private bool IsLoading { get; set; }
        private MudDataGrid<InvoiceModel> dataGrid;
        private string searchString;

        private List<InvoiceModel> InvoiceRequestModel = new List<InvoiceModel>();
        private bool isViewOnly = false;
        private bool isBigThreeRoles = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isBigThreeRoles = TokenHelper.IsBigThreeRolesWithoutSupervisor(await AuthState);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task ReloadRequestModel()
        {
            try
            {
                var dataList = await InvoiceService.GetAllInvoiceAsync();

                if (dataList == null)
                {
                    IsLoading = false;
                    return;
                }

                var depositList = await DepositService.GetAllDepositAsync();

                IMapper mapper = MappingWebHelper.InitializeMapper();

                foreach (var ul in dataList)
                {
                    List<DepositDTO> depositData = new List<DepositDTO>();

                    if (depositList != null)
                        depositData = depositList.Where(x => x.JobOrderId == ul.JobOrder.Id).ToList();

                    Color statusColor = Color.Primary;
                    if (ul.JobStatus.Name.Equals(Constants.JobStatus.Open))
                        statusColor = Color.Warning;
                    else if (ul.JobStatus.Name.Equals(Constants.JobStatus.Completed))
                        statusColor = Color.Success;
                    else if (ul.JobStatus.Name.Equals(Constants.JobStatus.Cancelled))
                        statusColor = Color.Error;

                    var customerMap = mapper.Map<CustomerModel>(ul.Customer);
                    var jobStatusMap = ul.JobStatus.Map<JobStatusModel>();

                    InvoiceRequestModel.Add(new InvoiceModel()
                    {
                        StatusChipColor = statusColor,
                        Id = ul.Id,
                        IsPackage = ul.IsPackage,
                        InvoiceNo = ul.InvoiceNo,
                        InvoiceDate = ul.InvoiceDate,
                        DueDate = ul.DueDate,

                        JobOrder = new JobOrderModel()
                        {
                            Id = ul.JobOrder.Id,
                            ReferenceNo = ul.JobOrder.ReferenceNo
                        },

                        JobStatus = jobStatusMap,
                        Customer = customerMap,
                        CustomerPO = ul.CustomerPO,

                        SubTotal = ul.SubTotal,
                        TotalAmount = ul.TotalAmount,
                        DepositAmount = depositData.Sum(x => x.DepositAmount)
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

        private async Task<GridData<InvoiceModel>> ServerReload(GridState<InvoiceModel> state)
        {
            if (!InvoiceRequestModel.Any())
                await ReloadRequestModel();

            IEnumerable<InvoiceModel> data = new List<InvoiceModel>();
            data = InvoiceRequestModel.OrderByDescending(x => x.InvoiceDate);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.InvoiceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Customer.FirstName} {element.Customer.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.JobOrder.ReferenceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.DueDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.TotalAmount.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.InvoiceDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
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
                    case nameof(InvoiceModel.InvoiceNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.InvoiceNo
                        );
                        break;
                    case nameof(InvoiceModel.Customer):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Customer.FirstName
                        );
                        break;
                    case nameof(InvoiceModel.JobOrder):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.JobOrder.ReferenceNo
                        );
                        break;
                    case nameof(InvoiceModel.DueDate):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.DueDate
                        );
                        break;
                    case nameof(InvoiceModel.InvoiceDate):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.InvoiceDate
                        );
                        break;
                    case nameof(InvoiceModel.TotalAmount):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.TotalAmount
                        );
                        break;
                    case nameof(InvoiceModel.DepositAmount):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.DepositAmount
                        );
                        break;

                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<InvoiceModel>
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
            NavigationManager.NavigateToCustom("/operations/invoices/add");
        }

        private async Task OnDeleteClick(InvoiceModel model)
        {
            try
            {
                // validate if model status is not OPEN then prevent deletion
                if (!model.JobStatus.Name.Equals(Constants.JobStatus.Open))
                {
                    mBoxCustomMessage = "Completed / Converted invoice cannot be deleted.";
                    await mboxError.ShowAsync();

                    return;
                }

                if (model != null)
                {
                    bool? result = await mbox.ShowAsync();
                    var proceed = result == null ? false : true;

                    if (proceed)
                    {
                        IsLoading = true;

                        var invoicePackageToDelete = await InvoicePackageService.GetAllInvoicePackageByInvoiceIdAsync(model.Id);
                        if (invoicePackageToDelete != null)
                        {
                            foreach (var ip in invoicePackageToDelete)
                            {
                                await InvoicePackageService.DeleteAsync(ip.Id);
                            }
                        }

                        await InvoiceService.DeleteAsync(model.Id);
                        SnackbarService.Add("Invoice Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/operations/invoices", true);
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

        private async Task OnGeneratePdfClick(int invoiceId)
        {
            IsLoading = true;

            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            var InvoiceRequestModel = await InvoiceService.GetInvoiceByIdAsync(invoiceId);
            var invoiceInfo = InvoiceRequestModel;

            invoiceInfo.PackageList = await InvoicePackageService.GetAllInvoicePackageByInvoiceIdAsync(invoiceInfo.Id);
            invoiceInfo.ProductList = await JobOrderProductService.GetAllJobOrderProductByJobOrderIdAsync(invoiceInfo.JobOrder.Id);
            invoiceInfo.ServiceList = await JobOrderServiceService.GetAllJobOrderServiceByJobOrderIdAsync(invoiceInfo.JobOrder.Id);
            invoiceInfo.TechnicianList = await JobOrderTechnicianService.GetAllJobOrderTechnicianByJobOrderIdAsync(invoiceInfo.JobOrder.Id);

            // payment information
            var PaymentRequestModel = new PaymentDTO();

            InvoiceReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            InvoiceReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
            await InvoiceReportGenerator.Generate(invoiceInfo, JSRuntime, PaymentRequestModel, companyData);

            IsLoading = false;

        }
    }
}