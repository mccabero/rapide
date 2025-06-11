using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class ServiceComponent
    {
        #region Parameters
        [Parameter]
        public string? ServicesId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IServiceService ServiceService { get; set; }
        [Inject]
        private IServiceGroupService ServiceGroupService { get; set; }
        [Inject]
        private IServiceCategoryService ServiceCategoryService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private ServiceDTO ServiceRequestModel { get; set; } = new();
        private List<ServiceCategoryDTO> ServiceCategoryList { get; set; } = new();
        private List<ServiceGroupDTO> ServiceGroupList { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(ServicesId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var allServiceGroup = await ServiceGroupService.GetAllAsync();
            var allServiceCategory = await ServiceCategoryService.GetAllAsync();

            ServiceCategoryList = allServiceCategory.ToList();
            ServiceGroupList = allServiceGroup.ToList();

            if (isEditMode)
            {
                // Get data by id
                ServiceRequestModel = await ServiceService.GetServiceByIdAsync(int.Parse(ServicesId));
                form.Disabled = isViewOnly;
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
                    bool isEditMode = !string.IsNullOrEmpty(ServicesId);

                    ServiceRequestModel.ServiceCategoryId = ServiceRequestModel.ServiceCategory.Id;
                    ServiceRequestModel.ServiceGroupId = ServiceRequestModel.ServiceGroup.Id;

                    if (!isEditMode) // create mode
                    {
                        ServiceRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ServiceRequestModel.CreatedDateTime = DateTime.Now;
                        ServiceRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ServiceRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await ServiceService.CreateAsync(ServiceRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Service Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/services");

                    }
                    else // update mode
                    {
                        ServiceRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        ServiceRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await ServiceService.UpdateAsync(ServiceRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Service Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/services");
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
            NavigationManager.NavigateToCustom("/configurations/services");
        }

        private void OnAddNewItemClick(ServiceDialogType dialogType)
        {
            bool isEditMode = !string.IsNullOrEmpty(ServicesId);
            var returnUrl = isEditMode
                ? $"/configurations/services/{ServicesId}"
                : "/configurations/services/add";

            if (dialogType == ServiceDialogType.Group)
                NavigationManager.NavigateToCustom($"/configurations/service-groups/add?returnUrl={returnUrl}");
            else if (dialogType == ServiceDialogType.Category)
                NavigationManager.NavigateToCustom($"/configurations/service-categories/add?returnUrl={returnUrl}");
        }

        private async Task<IEnumerable<ServiceGroupDTO>> SearchServiceGroup(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ServiceGroupList;

            return ServiceGroupList.Where(i => $"{i.Name} {i.Description}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ServiceCategoryDTO>> SearchServiceCategory(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ServiceCategoryList;

            return ServiceCategoryList.Where(i => $"{i.Name} {i.Description}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}