using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Services;
using Rapide.Web.Helpers;
using System.Text.RegularExpressions;
using static Rapide.Web.Components.Pages.Administrator.UserDetails;

namespace Rapide.Web.Components.Pages.Administrator
{
    public class RoleDropItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Place { get; set; }
    }

    public partial class UserDetails
    {
        #region Parameter
        [Parameter]
        public string? UserId { get; set; }
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        [Inject]
        private IRoleService RoleService { get; set; }
        [Inject]
        private IUserRolesService UserRolesService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private List<RoleDTO> Roles { get; set; } = new();
        private MudMessageBox mbox { get; set; }
        private UserDTO UserRequestModel { get; set; } = new();
        private bool IsLoading { get; set; }
        private bool IsError { get; set; } = false;
        private string ErrorMessage { get; set; } = string.Empty;

        private MudForm form;
        private string[] errors = { };
        private bool success;
        private MudTextField<string> pwField1;
        private List<RoleDropItem> roleItems = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            ErrorMessage = string.Empty;
            IsError = false;

            IsLoading = true;
            Roles = await RoleService.GetAllAsync();
            ReloadAllUserRoles();

            bool isEditMode = !string.IsNullOrEmpty(UserId);

            if (isEditMode)
            {
                // Get user data by id
                UserRequestModel = await UserService.GetUserRoleByIdAsync(int.Parse(UserId));
                UserRequestModel.PasswordHash = CryptographyHelper.Decrypt(UserRequestModel.PasswordHash, CryptographyHelper.GetEncryptionKey());
                UserRequestModel.ConfirmPasswordHash = UserRequestModel.PasswordHash;

                UserRequestModel.UserRoles = await UserRolesService.GetUserRolesByUserIdAsync(int.Parse(UserId));

                if (UserRequestModel.UserRoles != null)
                {
                    foreach (var ur in UserRequestModel.UserRoles)
                    {
                        var multipleRoles = roleItems.Where(x => x.Id == ur.RoleId);
                        if (multipleRoles != null && multipleRoles.Any())
                        {
                            roleItems.Where(x => x.Id == multipleRoles.FirstOrDefault().Id).FirstOrDefault().Place = "Assigned";
                        }
                    }
                }


                var assignedRoles = roleItems.Where(x => x.Id == UserRequestModel.RoleId);
                if (assignedRoles != null && assignedRoles.Any())
                {
                    roleItems.Where(x => x.Id == assignedRoles.FirstOrDefault().Id).FirstOrDefault().Place = "Assigned";
                }

            }
            else 
            {
                UserRequestModel.Birthday = DateTime.Now;
            }

            IsLoading = false;
            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private void ReloadAllUserRoles()
        {
            roleItems = new List<RoleDropItem>();
            foreach (var r in Roles)
            {
                roleItems.Add(new RoleDropItem()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Place = "Available"
                });
            }
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
                    UserRequestModel.RoleId = UserRequestModel.Role.Id;

                    #region Validation
                    if (string.IsNullOrEmpty(UserRequestModel.PasswordHash) || string.IsNullOrEmpty(UserRequestModel.ConfirmPasswordHash))
                    {
                        ErrorMessage = "Password or confirm password field is empty.";
                        IsError = true;

                        return;
                    }

                    var isRequiredFieldPassed = ValidateRequiredField();
                    var isConfirmPasswordPassed = ValidateConfirmPassword();

                    if (!isRequiredFieldPassed)
                    {
                        ErrorMessage = "Required field is empty.";
                        IsError = true;
                        
                        return;
                    }
                    if (!isConfirmPasswordPassed)
                    {
                        ErrorMessage = "Password and Confirm Password is not matched!";
                        IsError = true;

                        return;
                    }
                    #endregion

                    IsLoading = true;
                    bool isEditMode = !string.IsNullOrEmpty(UserId);
                    
                    UserRequestModel.Salt = Rapide.Common.Helpers.CryptographyHelper.GenerateSalt();
                    UserRequestModel.PasswordHash =
                        Rapide.Common.Helpers.CryptographyHelper.Encrypt(
                            UserRequestModel.PasswordHash,
                            Rapide.Common.Helpers.CryptographyHelper.GetEncryptionKey());

                    if (!isEditMode) // create mode
                    {
                        UserRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        UserRequestModel.CreatedDateTime = DateTime.Now;
                        UserRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        UserRequestModel.UpdatedDateTime = DateTime.Now;


                        // call create endpoint here...
                        var created = await UserService.CreateAsync(UserRequestModel);

                        // Save roles
                        var assignedRoles = roleItems.Where(x => x.Place == "Assigned");
                        foreach (var p in assignedRoles)
                        {
                            var ur = new UserRolesDTO();

                            ur.UserId = created.Id;
                            ur.RoleId = p.Id;

                            ur.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            ur.CreatedDateTime = DateTime.Now;
                            ur.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            ur.UpdatedDateTime = DateTime.Now;

                            await UserRolesService.CreateAsync(ur);
                        }

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("User Account Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/administrators/users");
                        
                    }
                    else // update mode
                    {
                        int userId = int.Parse(UserId);

                        UserRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        UserRequestModel.UpdatedDateTime = DateTime.Now;

                        UserRequestModel.Role = null;

                        // call update endpoint here...
                        await UserService.UpdateAsync(UserRequestModel);

                        // Detele all current services? Update also include insert inside
                        var userRolesByUser = await UserRolesService.GetUserRolesByUserIdAsync(userId);

                        if (userRolesByUser != null && userRolesByUser.Any())
                        {
                            foreach (var del in userRolesByUser)
                            {
                                await UserRolesService.DeleteAsync(del.Id);
                            }
                        }

                        var assignedRoles = roleItems.Where(x => x.Place == "Assigned");
                        foreach (var p in assignedRoles)
                        {
                            var ur = new UserRolesDTO();

                            ur.UserId = userId;
                            ur.RoleId = p.Id;

                            ur.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            ur.CreatedDateTime = DateTime.Now;
                            ur.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                            ur.UpdatedDateTime = DateTime.Now;

                            await UserRolesService.CreateAsync(ur);
                        }


                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("User Account Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/administrators/users");
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
            NavigationManager.NavigateToCustom("/administrators/users");
        }

        private bool ValidateRequiredField()
        {
            var user = UserRequestModel;

            if (user.RoleId == 0)
                return false;

            if (string.IsNullOrEmpty(user.FirstName)
                || string.IsNullOrEmpty(user.LastName)
                || string.IsNullOrEmpty(user.Email)
                || string.IsNullOrEmpty(user.PasswordHash)
                || string.IsNullOrEmpty(user.ConfirmPasswordHash))
            {
                return false;
            }
            
            return true;
        }

        private bool ValidateConfirmPassword()
        {
            return UserRequestModel.PasswordHash
                .Equals(UserRequestModel.ConfirmPasswordHash);
        }

        private IEnumerable<string> PasswordStrength(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw))
            {
                yield return "Password is required!";
                yield break;
            }
            if (pw.Length < 8)
                yield return "Password must be at least of length 8";
            if (!Regex.IsMatch(pw, @"[A-Z]"))
                yield return "Password must contain at least one capital letter";
            if (!Regex.IsMatch(pw, @"[a-z]"))
                yield return "Password must contain at least one lowercase letter";
            if (!Regex.IsMatch(pw, @"[0-9]"))
                yield return "Password must contain at least one digit";
        }

        private string PasswordMatch(string arg)
        {
            if (pwField1.Value != arg)
                return "Passwords don't match";
            return null;
        }

        private void OnAddNewItemClick()
        {
            bool isEditMode = !string.IsNullOrEmpty(UserId);
            var returnUrl = isEditMode
                ? $"/administrators/users/{UserId}"
                : "/administrators/users/add";

            NavigationManager.NavigateToCustom($"/administrators/user-roles/add?returnUrl={returnUrl}");
        }

        private async Task<IEnumerable<RoleDTO>> SearchRole(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return Roles;

            return Roles.Where(i => i.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private void ItemUpdated(MudItemDropInfo<RoleDropItem> dropItem)
        {
            dropItem.Item.Place = dropItem.DropzoneIdentifier;
        }

        private async Task OnUserRoleChanged(UserDTO element, RoleDTO dto)
        {
            ReloadAllUserRoles();
            UserRequestModel.Role = dto;
            
            var assignedRoles = roleItems.Where(x => x.Id == dto.Id);
            if (assignedRoles != null && assignedRoles.Any())
            {
                roleItems.Where(x => x.Id == assignedRoles.FirstOrDefault().Id).FirstOrDefault().Place = "Assigned";
            }

            StateHasChanged();
        }
    }
}
