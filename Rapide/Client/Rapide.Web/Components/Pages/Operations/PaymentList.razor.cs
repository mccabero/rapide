using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.PdfReportGenerator;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class PaymentList
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
        private IPaymentService PaymentService { get; set; }
        [Inject]
        private IPaymentDetailsService PaymentDetailsService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        [Inject]
        private IInvoiceService InvoiceService { get; set; }
        [Inject]
        private IInvoicePackageService InvoicePackageService { get; set; }
        [Inject]
        private IJobOrderProductService JobOrderProductService { get; set; }
        [Inject]
        private IJobOrderServiceService JobOrderServiceService { get; set; }
        [Inject]
        private IJobOrderTechnicianService JobOrderTechnicianService { get; set; }
        [Inject]
        private IJobStatusService JobStatusService { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private MudMessageBox mboxError { get; set; }
        private string mBoxCustomMessage { get; set; }
        private bool IsLoading { get; set; }

        private List<JobStatusDTO> JobStatusList { get; set; } = new();

        private MudDataGrid<PaymentModel> dataGrid;
        private string searchString;

        private List<PaymentModel> PaymentRequestModel = new List<PaymentModel>();
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            JobStatusList = await JobStatusService.GetAllAsync();

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task ReloadRequestModel()
        {
            try
            {
                var dataList = await PaymentService.GetAllPaymentAsync();

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
                        statusColor = Color.Info;
                    else if (ul.JobStatus.Name.Equals(Constants.JobStatus.Deleted))
                        statusColor = Color.Error;

                    PaymentRequestModel.Add(new PaymentModel()
                    {
                        IsAllowedToOverride = TokenHelper.IsBigThreeRoles(await AuthState),
                        StatusChipColor = statusColor,
                        Id = ul.Id,
                        IsFullyPaid = ul.IsFullyPaid,
                        ReferenceNo = ul.ReferenceNo,
                        PaymentDate = ul.PaymentDate,
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
                        InvoiceTotalAmount = ul.InvoiceTotalAmount,
                        Remarks = ul.Remarks,
                        VAT12 = ul.VAT12,
                        DepositAmount = ul.DepositAmount,
                        AmountPayable = ul.AmountPayable,
                        TotalPaidAmount = ul.TotalPaidAmount,
                        Balance = ul.Balance
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

        private async Task<GridData<PaymentModel>> ServerReload(GridState<PaymentModel> state)
        {
            if (!PaymentRequestModel.Any())
                await ReloadRequestModel();

            IEnumerable<PaymentModel> data = new List<PaymentModel>();
            data = PaymentRequestModel.OrderByDescending(x => x.PaymentDate);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.ReferenceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Customer.FirstName} {element.Customer.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.AmountPayable.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.TotalPaidAmount.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.PaymentDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Balance.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.JobStatus.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(PaymentModel.ReferenceNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.ReferenceNo
                        );
                        break;
                    case nameof(PaymentModel.Customer.FirstName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Customer.FirstName
                        );
                        break;
                    case nameof(PaymentModel.AmountPayable):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.AmountPayable
                        );
                        break;
                    case nameof(PaymentModel.PaymentDate):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.PaymentDate
                        );
                        break;
                    case nameof(PaymentModel.Balance):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Balance
                        );
                        break;
                    case nameof(PaymentModel.TotalPaidAmount):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.TotalPaidAmount
                        );
                        break;

                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<PaymentModel>
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
            NavigationManager.NavigateToCustom("/operations/payments/add");
        }

        private async Task OnDeleteClick(PaymentModel model)
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

                        var jobStatusDeleted = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Deleted)).FirstOrDefault();

                        var dataToDelete = await PaymentService.GetPaymentByIdAsync(model.Id);
                        dataToDelete.JobStatus = jobStatusDeleted;
                        dataToDelete.JobStatusId = jobStatusDeleted.Id;

                        await PaymentService.UpdateAsync(dataToDelete);

                        SnackbarService.Add("Payment Successfuly Deleted!", Severity.Normal, config => { config.ShowCloseIcon = true; });

                        IsLoading = false;
                        StateHasChanged();

                        NavigationManager.NavigateToCustom("/operations/payments", true);
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

        private async Task OnGeneratePdfClick(int paymentId)
        {
            IsLoading = true;

            var PaymentRequestModel = await PaymentService.GetPaymentByIdAsync(paymentId);
            var paymentDetailsInfo = await PaymentDetailsService.GetAllPaymentDetailsByPaymentIdAsync(paymentId);

            if (paymentDetailsInfo == null)
            {
                IsLoading = false;
                StateHasChanged();

                mBoxCustomMessage = "Payment details is empty!";
                await mboxError.ShowAsync();
                
                return;
            }

            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            var InvoiceRequestModel = await InvoiceService.GetInvoiceByIdAsync(paymentDetailsInfo.FirstOrDefault().InvoiceId);
            var invoiceInfo = InvoiceRequestModel;

            invoiceInfo.PackageList = await InvoicePackageService.GetAllInvoicePackageByInvoiceIdAsync(invoiceInfo.Id);
            invoiceInfo.ProductList = await JobOrderProductService.GetAllJobOrderProductByJobOrderIdAsync(invoiceInfo.JobOrder.Id);
            invoiceInfo.ServiceList = await JobOrderServiceService.GetAllJobOrderServiceByJobOrderIdAsync(invoiceInfo.JobOrder.Id);
            invoiceInfo.TechnicianList = await JobOrderTechnicianService.GetAllJobOrderTechnicianByJobOrderIdAsync(invoiceInfo.JobOrder.Id);

            // payment information

            InvoiceReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            InvoiceReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
            await InvoiceReportGenerator.Generate(invoiceInfo, JSRuntime, PaymentRequestModel, companyData);

            IsLoading = false;

        }
    }
}