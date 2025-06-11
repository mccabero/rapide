using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.SystemConfiguration
{
    public partial class VehicleModelDetails
    {
        #region Parameters
        [Parameter]
        public string? VehicleModelId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IVehicleModelService VehicleModelService { get; set; }
        [Inject]
        private IVehicleMakeService VehicleMakeService { get; set; }
        [Inject]
        private IParameterService ParameterService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private VehicleModelDTO VehicleModelRequestModel { get; set; } = new();
        private List<ParameterDTO> BodyParameters { get; set; } = new();
        private List<ParameterDTO> ClassificationParameters { get; set; } = new();
        private List<VehicleMakeDTO> VehicleMakes { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(VehicleModelId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var allParams = await ParameterService.GetAllParameterAsync();

            BodyParameters = allParams
                .Where(x => x.ParameterGroup.Name.Contains(Constants.ParameterType.BodyTypeParam))
                .OrderBy(x => x.Name)
                .ToList();

            ClassificationParameters = allParams
                .Where(x => x.ParameterGroup.Name.Contains(Constants.ParameterType.ClassificationParam))
                .OrderBy(x => x.Name)
                .ToList();

            VehicleMakes = await VehicleMakeService.GetAllAsync();

            if (isEditMode)
            {
                // Get data by id
                VehicleModelRequestModel = await VehicleModelService.GetVehicleModelByIdAsync(int.Parse(VehicleModelId));
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
                    bool isEditMode = !string.IsNullOrEmpty(VehicleModelId);

                    VehicleModelRequestModel.VehicleMakeId = VehicleModelRequestModel.VehicleMake.Id;
                    
                    // 05Feb2025: Default to unknown since not needed for now.
                    var bodyTypeList = await ParameterService.GetAllParameterAsync();
                    var unknownParam = bodyTypeList.Where(x => x.Name.ToUpper() == "UNKNOWN").ToList();
                    var unknownBodyType = unknownParam.Where(x => x.ParameterGroup.Name.ToUpper() == "BODY TYPE").ToList();
                    var unknownClassification = unknownParam.Where(x => x.ParameterGroup.Name.ToUpper() == "CLASSIFICATION").ToList();

                    VehicleModelRequestModel.BodyParameterId = unknownBodyType.FirstOrDefault().Id;
                    VehicleModelRequestModel.ClassificationParameterId = unknownClassification.FirstOrDefault().Id;

                    if (!isEditMode) // create mode
                    {
                        VehicleModelRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        VehicleModelRequestModel.CreatedDateTime = DateTime.Now;
                        VehicleModelRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        VehicleModelRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await VehicleModelService.CreateAsync(VehicleModelRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Vehicle Model Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/vehicle-models");

                    }
                    else // update mode
                    {
                        VehicleModelRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        VehicleModelRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await VehicleModelService.UpdateAsync(VehicleModelRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Vehicle Model Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/vehicle-models");
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
            NavigationManager.NavigateToCustom("/configurations/vehicle-models");
        }

        private void OnAddNewItemClick(VehicleModelDialogType dialogType)
        {
            bool isEditMode = !string.IsNullOrEmpty(VehicleModelId);
            var returnUrl = isEditMode
                ? $"/configurations/vehicle-models/{VehicleModelId}"
                : "/configurations/vehicle-models/add";

            if (dialogType == VehicleModelDialogType.Make)
                NavigationManager.NavigateToCustom($"/configurations/vehicle-makes/add?returnUrl={returnUrl}");
            else if (dialogType == VehicleModelDialogType.Body)
                NavigationManager.NavigateToCustom($"/configurations/parameters/add?returnUrl={returnUrl}");
            else if (dialogType == VehicleModelDialogType.Classification)
                NavigationManager.NavigateToCustom($"/configurations/parameters/add?returnUrl={returnUrl}");
        }

        #region Search MudAutoComplete
        private async Task<IEnumerable<VehicleMakeDTO>> SearchVehicleMake(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return VehicleMakes;

            return VehicleMakes.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ParameterDTO>> SearchBody(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return BodyParameters;

            return BodyParameters.Where(i => $"{i.Name} {i.Description}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private async Task<IEnumerable<ParameterDTO>> SearchClassification(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return ClassificationParameters;

            return ClassificationParameters.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        #endregion
    }
}
