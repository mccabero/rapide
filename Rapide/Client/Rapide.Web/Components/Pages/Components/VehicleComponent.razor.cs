using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class VehicleComponent
    {
        #region Parameters
        [Parameter]
        public string? VehicleId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private ICustomerService CustomerService { get; set; }
        [Inject]
        private IVehicleService VehicleService { get; set; }
        [Inject]
        private IVehicleModelService VehicleModelService { get; set; }
        [Inject]
        private IParameterService ParameterService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IDialogService Dialog { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        #endregion

        #region Private Properties
        private VehicleDTO VehicleRequestModel { get; set; } = new();
        private List<CustomerDTO> CustomerList { get; set; } = new();
        private List<VehicleModelDTO> VehicleModelList { get; set; } = new();
        private List<ParameterDTO> TransmissionList { get; set; } = new();
        private List<ParameterDTO> EngineTypeList { get; set; } = new();
        private List<ParameterDTO> EngineSizeList { get; set; } = new();
        private List<ParameterDTO> OdometerTypeList { get; set; } = new();
        private List<ParameterDTO> CustomerRegistrationTypeList { get; set; } = new();

        private bool IsLoading { get; set; }
        private MudMessageBox mbox { get; set; }

        private readonly DialogOptions _fullScreen = new() { FullScreen = true, CloseButton = true };

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            bool isEditMode = !string.IsNullOrEmpty(VehicleId);

            var allParams = await ParameterService.GetAllParameterAsync();

            CustomerList = await CustomerService.GetAllAsync();
            VehicleModelList = await VehicleModelService.GetAllVehicleModelAsync();
            TransmissionList = allParams
                .Where(x => x.ParameterGroup.Name.Contains(Constants.ParameterType.TransmissionParam))
                .OrderBy(x => x.Name)
                .ToList();

            EngineTypeList = allParams
                .Where(x => x.ParameterGroup.Name.Contains(Constants.ParameterType.EngineTypeParam))
                .OrderBy(x => x.Name)
                .ToList();

            EngineSizeList = allParams
                .Where(x => x.ParameterGroup.Name.Contains(Constants.ParameterType.EngineSizeParam))
                .OrderBy(x => x.Name)
                .ToList();

            OdometerTypeList = allParams
                .Where(x => x.ParameterGroup.Name.Contains(Constants.ParameterType.OdometerTypeParam))
                .OrderBy(x => x.Name)
                .ToList();

            CustomerRegistrationTypeList = allParams
                .Where(x => x.ParameterGroup.Name.Contains(Constants.ParameterType.CustomerRegistrationTypeParam))
                .OrderBy(x => x.Name)
                .ToList();

            if (isEditMode)
            {
                // Get data by id
                VehicleRequestModel = await VehicleService.GetVehicleByIdAsync(int.Parse(VehicleId));
                form.Disabled = isViewOnly;
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
                    bool isEditMode = !string.IsNullOrEmpty(VehicleId);

                    VehicleRequestModel.CustomerId = VehicleRequestModel.Customer.Id;
                    VehicleRequestModel.VehicleModelId = VehicleRequestModel.VehicleModel.Id;
                    VehicleRequestModel.TransmissionParameterId = VehicleRequestModel.TransmissionParameter.Id;
                    VehicleRequestModel.EngineTypeParameterId = VehicleRequestModel.EngineTypeParameter.Id;
                    VehicleRequestModel.EngineSizeParameterId = VehicleRequestModel.EngineSizeParameter.Id;
                    VehicleRequestModel.OdometerParameterId = VehicleRequestModel.OdometerParameter.Id;
                    VehicleRequestModel.CustomerRegistrationTypeParameterId = VehicleRequestModel.CustomerRegistrationTypeParameter.Id;

                    if (!isEditMode) // create mode
                    {
                        VehicleRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        VehicleRequestModel.CreatedDateTime = DateTime.Now;
                        VehicleRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        VehicleRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await VehicleService.CreateAsync(VehicleRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Vehicle Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/vehicles");

                    }
                    else // update mode
                    {
                        VehicleRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        VehicleRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await VehicleService.UpdateAsync(VehicleRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Vehicle Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/vehicles");
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
            NavigationManager.NavigateToCustom("/vehicles");
        }

        private void OnAddNewItemClick(VehicleDialogType dialogType)
        {
            bool isEditMode = !string.IsNullOrEmpty(VehicleId);
            var returnUrl = isEditMode
                ? $"/vehicles/{VehicleId}"
                : "/vehicles/add";

            if (dialogType == VehicleDialogType.Customer)
                NavigationManager.NavigateTo($"/customers/add?returnUrl={returnUrl}");
            else if (dialogType == VehicleDialogType.Parameter)
                NavigationManager.NavigateTo($"/configurations/parameters/add?returnUrl={returnUrl}");
            else if (dialogType == VehicleDialogType.VehicleModel)
                NavigationManager.NavigateTo($"/configurations/vehicle-models/add?returnUrl={returnUrl}");
        }

        #region Search MudAutoComplete
        private async Task<IEnumerable<CustomerDTO>> SearchCustomer(string filter, CancellationToken token)
        {
            CustomerList = CustomerList.OrderByDescending(x => x.CreatedDateTime).ToList();
            if (string.IsNullOrEmpty(filter))
                return CustomerList;

            return CustomerList.Where(i => $"{i.FirstName} {i.MiddleName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<VehicleModelDTO>> SearchVehicleModel(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return VehicleModelList;

            return VehicleModelList.Where(i => $"{i.Name} {i.Description}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ParameterDTO>> SearchTransmission(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return TransmissionList;

            return TransmissionList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ParameterDTO>> SearchEngineType(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return EngineTypeList;

            return EngineTypeList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ParameterDTO>> SearchEngineSize(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return EngineSizeList;

            return EngineSizeList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ParameterDTO>> SearchOdometerType(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return OdometerTypeList;

            return OdometerTypeList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ParameterDTO>> SearchCustomerRegistrationType(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return CustomerRegistrationTypeList;

            return CustomerRegistrationTypeList.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
        #endregion

        // For Reference in the future.
        private Task OpenDialogAsync(DialogOptions options, VehicleDialogType dialogType)
        {
            return Dialog.ShowAsync<MudDialogComponent>(dialogType.ToString(), options);
        }
    }
}
