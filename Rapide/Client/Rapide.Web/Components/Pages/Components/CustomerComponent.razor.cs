using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class CustomerComponent : ComponentBase
    {
        #region Parameters
        [Parameter]
        public string? CustomerId { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private ICustomerService CustomerService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private MudMessageBox mboxError { get; set; }
        private CustomerDTO CustomerRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        private string mBoxCustomMessage { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            IsReadOnly = isViewOnly;

            bool isEditMode = !string.IsNullOrEmpty(CustomerId);
            
            if (isEditMode)
            {
                // Get data by id
                CustomerRequestModel = await CustomerService.GetByIdAsync(int.Parse(CustomerId));
                form.Disabled = IsReadOnly;
            }
            else
            {
                var data = await CustomerService.GetAllAsync();
                var lastId = 1;

                if (data != null)
                {
                    lastId = data.Count();
                    lastId++;
                }

                CustomerRequestModel.CustomerCode = $"CUST{lastId.ToString("0000000")}";
                CustomerRequestModel.IsActive = true;
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            
            base.OnAfterRender(firstRender);
        }

        private async Task OnSaveClick()
        {
            bool mobileHasCharacter = CustomerRequestModel.MobileNumber.Any(x => char.IsLetter(x));
            if (mobileHasCharacter)
            {
                mBoxCustomMessage = "Mobile number contains character. Please input valid mobile number!";
                await mboxError.ShowAsync();

                return;
            }

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
                    bool isEditMode = !string.IsNullOrEmpty(CustomerId);

                    if (!isEditMode) // create mode
                    {
                        CustomerRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        CustomerRequestModel.CreatedDateTime = DateTime.Now;
                        CustomerRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        CustomerRequestModel.UpdatedDateTime = DateTime.Now;

                        // call create endpoint here...
                        await CustomerService.CreateAsync(CustomerRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Customer Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/customers");

                    }
                    else // update mode
                    {
                        CustomerRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        CustomerRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await CustomerService.UpdateAsync(CustomerRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Customer Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/customers");
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
            NavigationManager.NavigateToCustom("/customers");
        }
    }
}