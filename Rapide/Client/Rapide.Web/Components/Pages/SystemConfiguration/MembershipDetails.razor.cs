using Castle.Core.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class MembershipDetails
    {
        #region Parameter
        [Parameter]
        public string MembershipId { get; set; }
        #endregion

        #region DependencyInjection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IMembershipService MembershipService { get; set; }
        [Inject]
        private ICustomerService CustomerService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private List<CustomerDTO> CustomerList { get; set; } = new();
        public MudMessageBox mbox { get; set; }
        private MembershipDTO MembershipRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(MembershipId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);
            MembershipRequestModel.Customer = new CustomerDTO();

            if (isEditMode)
            {
                MembershipRequestModel = await MembershipService.GetAsync(x => x.Id == int.Parse(MembershipId));
                MembershipRequestModel.Customer = await CustomerService.GetByIdAsync(MembershipRequestModel.CustomerId);
                form.Disabled = isViewOnly;
            }
            else
            {
                MembershipRequestModel.MembershipDate = DateTime.Now;
                MembershipRequestModel.ExpiryDate = MembershipRequestModel.MembershipDate?.AddYears(1);
                
                CustomerList = await CustomerService.GetAllAsync();
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private async Task<IEnumerable<CustomerDTO>> SearchCustomer(string filter, CancellationToken token)
        {
            CustomerList = CustomerList.OrderByDescending(x => x.CreatedDateTime).ToList();

            if (string.IsNullOrEmpty(filter))
                return CustomerList;

            return CustomerList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task OnCustomerChanged(CustomerDTO dto)
        {
            MembershipRequestModel.Customer = dto;
            MembershipRequestModel.Customer.FirstName = dto.FirstName;
            MembershipRequestModel.Customer.MiddleName = dto.MiddleName;
            MembershipRequestModel.Customer.LastName = dto.LastName;
            MembershipRequestModel.Customer.Birthday = dto.Birthday;
            MembershipRequestModel.Customer.MobileNumber = dto.MobileNumber;
            MembershipRequestModel.Customer.Email = dto.Email;
            MembershipRequestModel.Customer.HomeAddress = dto.HomeAddress;

            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            bool isEditMode = string.IsNullOrEmpty(MembershipId);
            var returnUrl = isEditMode
                ? $"/configurations/memberships/{MembershipId}"
                : "/configurations/memberships/add";

            NavigationManager.NavigateToCustom($"/customers/add?returnUrl={returnUrl}");
        }

        private async Task OnSaveClick()
        {
            await form.Validate();
            if (!form.IsValid)
                return;

            bool? result = await mbox.ShowAsync();
            var proceedsaving = result == null ? false : true;

            if (proceedsaving)
            {
                try
                {
                    IsLoading = true;
                    bool isEditMode = !string.IsNullOrEmpty(MembershipId);

                    if (!isEditMode)
                    {
                        MembershipRequestModel.CustomerId = MembershipRequestModel.Customer.Id;
                        MembershipRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        MembershipRequestModel.CreatedDateTime = DateTime.Now;
                        MembershipRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        MembershipRequestModel.UpdatedDateTime = DateTime.Now;

                        MembershipRequestModel.MembershipNo = "temp";

                        var savedMembership = await MembershipService.CreateAsync(MembershipRequestModel);

                        savedMembership.MembershipNo = $"M{savedMembership.Id.ToString("D7")}";

                        await MembershipService.UpdateAsync(savedMembership);

                        IsLoading = false;

                        SnackbarService.Add("Membership Successfully Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/memberships");
                    }
                    else
                    {
                        MembershipRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        MembershipRequestModel.UpdatedDateTime = DateTime.Now;

                        await MembershipService.UpdateAsync(MembershipRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Membership Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/memberships");
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
                    throw;
                }
            }
        }
        
        private async Task OnCancelClick()
        {
            NavigationManager.NavigateToCustom("/configurations/memberships");
        }
    }
}
