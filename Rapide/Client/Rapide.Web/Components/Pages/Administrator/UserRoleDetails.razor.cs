using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Administrator
{
    public partial class UserRoleDetails
    {
        #region Parameters
        [Parameter]
        public string? UserRoleId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IRoleService RoleService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private MudMessageBox mbox { get; set; }
        private RoleDTO RoleRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }
        private bool IsError { get; set; } = false;
        private string ErrorMessage { get; set; } = string.Empty;

        private MudForm form;
        private string[] errors = { };
        private bool success;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            ErrorMessage = string.Empty;
            IsError = false;

            IsLoading = true;
            bool isEditMode = !string.IsNullOrEmpty(UserRoleId);

            if (isEditMode)
            {
                // Get data by id
                RoleRequestModel = await RoleService.GetAsync(x => x.Id == int.Parse(UserRoleId));
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
                    if (string.IsNullOrEmpty(RoleRequestModel.Name))
                    {
                        ErrorMessage = "Role name is required!";
                        IsError = true;
                        
                        return;
                    }

                    IsLoading = true;
                    bool isEditMode = !string.IsNullOrEmpty(UserRoleId);

                    if (!isEditMode) // create mode
                    {
                        RoleRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        RoleRequestModel.CreatedDateTime = DateTime.Now;
                        RoleRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        RoleRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        await RoleService.CreateAsync(RoleRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("User Role Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/administrators/user-roles");

                    }
                    else // update mode
                    {
                        RoleRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        RoleRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await RoleService.UpdateAsync(RoleRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("User Role Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/administrators/user-roles");
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
            NavigationManager.NavigateToCustom("/administrators/user-roles");
        }
    }
}