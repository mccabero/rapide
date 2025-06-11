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
    public partial class VehicleMakeDetails
    {
        #region Parameters
        [Parameter]
        public string? VehicleMakeId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
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
        private VehicleMakeDTO VehicleMakeRequestModel { get; set; } = new();
        private List<ParameterDTO> Parameters { get; set; } = new();
        private bool IsLoading { get; set; }

        private MudForm form;
        private string[] errors = { };
        private bool success;

        private bool isViewOnly = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(VehicleMakeId);
            isViewOnly = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.HR)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.Accountant);

            var allParams = await ParameterService.GetAllParameterAsync();
            Parameters = allParams
                .Where(x => x.ParameterGroup.Name.Contains(Constants.ParameterType.RegionParam))
                .OrderBy(x => x.Name)
                .ToList();

            if (isEditMode)
            {
                // Get data by id
                VehicleMakeRequestModel = await VehicleMakeService.GetVehicleMakeByIdAsync(int.Parse(VehicleMakeId));
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
                    bool isEditMode = !string.IsNullOrEmpty(VehicleMakeId);

                    VehicleMakeRequestModel.RegionParameterId = VehicleMakeRequestModel.RegionParameter.Id;
                    VehicleMakeRequestModel.RegionParameter.ParameterGroupId = VehicleMakeRequestModel.RegionParameter.ParameterGroup.Id;

                    if (!isEditMode) // create mode
                    {
                        VehicleMakeRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        VehicleMakeRequestModel.CreatedDateTime = DateTime.Now;
                        VehicleMakeRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        VehicleMakeRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await VehicleMakeService.CreateAsync(VehicleMakeRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Vehicle Make Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/vehicle-makes");

                    }
                    else // update mode
                    {
                        VehicleMakeRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        VehicleMakeRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await VehicleMakeService.UpdateAsync(VehicleMakeRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Vehicle Make Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/configurations/vehicle-makes");
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
            NavigationManager.NavigateToCustom("/configurations/vehicle-makes");
        }

        private void OnAddNewItemClick()
        {
            bool isEditMode = !string.IsNullOrEmpty(VehicleMakeId);
            var returnUrl = isEditMode
                ? $"/configurations/vehicle-makes/{VehicleMakeId}"
                : "/configurations/vehicle-makes/add";

            NavigationManager.NavigateToCustom($"/configurations/parameters/add?returnUrl={returnUrl}");
        }

        private async Task<IEnumerable<ParameterDTO>> SearchGroup(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return Parameters;

            return Parameters.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}
