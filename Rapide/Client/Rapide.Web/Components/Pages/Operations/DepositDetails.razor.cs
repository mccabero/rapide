using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.PdfReportGenerator;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class DepositDetails
    {
        #region Parameters
        [Parameter]
        public string? DepositId { get; set; }
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
        private IJobStatusService JobStatusService { get; set; }
        [Inject]
        private IParameterService ParameterService { get; set; }
        [Inject]
        private IDepositService DepositService { get; set; }
        [Inject]
        private ICustomerService CustomerService { get; set; }
        [Inject]
        private IJobOrderService JobOrderService { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
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

        private DepositDTO DepositRequestModel { get; set; } = new();

        private List<CustomerDTO> CustomerList { get; set; } = new();
        private List<ParameterDTO> PaymentTypeList { get; set; } = new();
        private List<JobStatusDTO> JobStatusList { get; set; } = new();
        private List<JobOrderDTO> JobOrderList { get; set; } = new();

        private bool isDepositLocked = false;
        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);


            var customerList = await CustomerService.GetAllAsync();
            var paymentTypeList = await ParameterService.GetAllParameterAsync();
            
            //JobOrderList = await JobOrderService.GetAllJobOrderAsync();
            JobStatusList = await JobStatusService.GetAllAsync();
            CustomerList = customerList;
            PaymentTypeList = paymentTypeList.Where(x => x.ParameterGroup.Name.Equals(Constants.ParameterType.PaymentTypeParam)).ToList();

            IsEditMode = !string.IsNullOrEmpty(DepositId);

            if (IsEditMode)
            {
                await ReloadDepositData();
                isDepositLocked = DepositRequestModel.JobStatus.Name.Equals(Constants.JobStatus.Completed) ||
                    isViewOnly;

                form.Disabled = isDepositLocked;
            }
            else
            {
                DepositRequestModel.JobStatus = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                DepositRequestModel.TransactionDateTime = DateTime.Now;
                DepositRequestModel.RefundDateTime = DateTime.Now;

                DepositRequestModel.ReferenceNo = await ReferenceNumberHelper.GetRNDeposit(DepositService);
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
                    bool isEditMode = !string.IsNullOrEmpty(DepositId);

                    DepositRequestModel.JobStatusId = DepositRequestModel.JobStatus.Id;
                    DepositRequestModel.CustomerId = DepositRequestModel.Customer.Id;
                    DepositRequestModel.JobOrderId = DepositRequestModel.JobOrder.Id;
                    DepositRequestModel.PaymentTypeParameterId = DepositRequestModel.PaymentTypeParameter.Id;


                    if (!isEditMode) // create mode
                    {
                        DepositRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        DepositRequestModel.CreatedDateTime = DateTime.Now;
                        DepositRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        DepositRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        DepositRequestModel.JobStatus = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Open)).FirstOrDefault();
                        DepositRequestModel.JobStatusId = DepositRequestModel.JobStatus.Id;

                        var created = await DepositService.CreateAsync(DepositRequestModel);

                        DepositId = created.Id.ToString();
                        await ReloadDepositData();

                        IsLoading = false;
                        SnackbarService.Add("Deposit Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/deposits", true);

                    }
                    else // update mode
                    {
                        int estimateId = int.Parse(DepositId);

                        DepositRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        DepositRequestModel.UpdatedDateTime = DateTime.Now;

                        await DepositService.UpdateAsync(DepositRequestModel);

                        await ReloadDepositData();

                        SnackbarService.Add("Deposit Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/operations/deposits", true);
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

        private async Task OnCancelClick()
        {
            NavigationManager.NavigateToCustom("/operations/deposits");
        }

        private async Task OnNewDepositClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the current transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
                NavigationManager.NavigateToCustom("/operations/deposits/add", true);
        }

        private async Task OnCancelDepositClick()
        {
            mBoxCustomMessage = "Are you sure you want to cancel the this transaction?";

            bool? result = await mboxCustom.ShowAsync();
            var proceedAddNew = result == null ? false : true;

            if (proceedAddNew)
            {
                var jobStatusCancelled = JobStatusList.Where(x => x.Name.Equals(Constants.JobStatus.Cancelled)).FirstOrDefault();
                DepositRequestModel.JobStatus = jobStatusCancelled;
                DepositRequestModel.JobStatusId = jobStatusCancelled.Id;

                await DepositService.UpdateAsync(DepositRequestModel);

                SnackbarService.Add("Deposit Successfuly Cancelled!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                NavigationManager.NavigateToCustom("/operations/deposits", true);
            }
        }

        private async Task ReloadDepositData()
        {
            // Get data by id
            DepositRequestModel = await DepositService.GetDepositByIdAsync(int.Parse(DepositId));
        }

        private async Task OnCustomerChanged(DepositDTO dto, CustomerDTO i)
        {
            DepositRequestModel.Customer = i;
            var jobOrders = await JobOrderService.GetAllJobOrderByCustomerIdAsync(i.Id);

            JobOrderList = jobOrders;

            StateHasChanged();
        }

        private void OnAddNewItemClick(DialogTypes dt)
        {
            bool isEditMode = !string.IsNullOrEmpty(DepositId);
            var returnUrl = isEditMode
                ? $"/operations/deposits/{DepositId}"
                : "/operations/deposits/add";

            if (dt == DialogTypes.Payment)
                NavigationManager.NavigateToCustom($"/configurations/parameters/add?returnUrl={returnUrl}");
            if (dt == DialogTypes.User)
                NavigationManager.NavigateToCustom($"/administrators/users/add?returnUrl={returnUrl}");
        }

        #region Search MudAutoComplete
        private async Task<IEnumerable<CustomerDTO>> SearchCustomer(string filter, CancellationToken token)
        {
            CustomerList = CustomerList.OrderByDescending(x => x.CreatedDateTime).ToList();

            if (string.IsNullOrEmpty(filter))
                return CustomerList;

            return CustomerList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
        private async Task<IEnumerable<ParameterDTO>> SearchPaymentType(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return PaymentTypeList;

            return PaymentTypeList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<JobOrderDTO>> SearchJobOrder(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return JobOrderList;

            return JobOrderList.Where(i => i.ReferenceNo.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<JobStatusDTO>> SearchJobStatus(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return JobStatusList;

            return JobStatusList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
        #endregion

        private async Task OnGeneratePdfClick()
        {
            var companyInfo = await CompanyInfoService.GetAllAsync();
            var companyData = (companyInfo == null && !companyInfo.Any())
                ? new()
                : companyInfo.FirstOrDefault();

            var depositRequestModel = await DepositService.GetDepositByIdAsync(int.Parse(DepositId));

            if (depositRequestModel == null)
                return;

            depositRequestModel.PreparedBy = await UserService.GetUserRoleByIdAsync(depositRequestModel.UpdatedById);

            DepositReportGenerator.ImageFile = FileHelper.GetRapideLogo();
            DepositReportGenerator.ImageFileCompany = FileHelper.GetCompanyLogo();
            await DepositReportGenerator.Generate(depositRequestModel, JSRuntime, companyData);
        }
    }
}