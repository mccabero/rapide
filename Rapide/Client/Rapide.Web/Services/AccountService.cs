using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using Rapide.Web.Models;
using Rapide.Web.StateManagement;
using System.Net.Http.Headers;

namespace Rapide.Web.Services
{
    public sealed class AccountService(
        HttpClient httpClient, 
        ILocalStorageService localStorage, 
        AuthenticationStateProvider authStateProvider,
        IUserService userService,
        IUserRolesService userRolesService,
        IJSRuntime jsRuntime) : IDisposable
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILocalStorageService _localStorage = localStorage;
        private readonly AuthenticationStateProvider _authStateProvider = authStateProvider;
        private readonly IJSRuntime JsRuntime = jsRuntime;
        private readonly IUserService _userService = userService;
        private readonly IUserRolesService _userRolesService = userRolesService;

        private bool isDisposed = false;

        public void Dispose()
        {
            isDisposed = true;
        }

        public async Task<LoginResponseModel> Login(LoginRequestModel loginRequest)
        {
            try
            {
                // Login flow here...
                var userByEmail = await _userService.GetUserRoleByEmailAsync(loginRequest.Email);

                if (userByEmail == null)
                {
                    return new LoginResponseModel()
                    {
                        ErrorMessage = "User with email not found!"
                    };
                }

                // Check if password is match
                var encryptedPassword = CryptographyHelper.Encrypt(loginRequest.Password, CryptographyHelper.GetEncryptionKey());

                if (userByEmail.PasswordHash != encryptedPassword)
                {
                    return new LoginResponseModel()
                    {
                        ErrorMessage = "Password is invalid!"
                    };
                }

                // list of roles
                userByEmail.UserRoles = await _userRolesService.GetUserRolesByUserIdAsync(userByEmail.Id);

                if (userByEmail.UserRoles == null || !userByEmail.UserRoles.Any())
                {
                    return new LoginResponseModel()
                    {
                        ErrorMessage = "No user roles found for this user. Please contact your supervisor or systems administrator!"
                    };
                }

                if (userByEmail.UserRoles.Where(x => x.Role.Name.Equals(Constants.UserRoles.JuniorTechnician)).Any()
                    || userByEmail.UserRoles.Where(x => x.Role.Name.Equals(Constants.UserRoles.SeniorTechnician)).Any())
                {
                    return new LoginResponseModel()
                    {
                        ErrorMessage = "Technician account is not allowed to access the application!"
                    };
                }

                var result = await TokenHelper.GenerateClaimsResponse(userByEmail);

                if (!isDisposed)
                {
                    await _localStorage.SetItemAsync(Constants.LocalUserDetails, result.User);
                    await _localStorage.SetItemAsStringAsync(Constants.LocalToken, result.Token);
                }
                

                ((AuthStateProvider)_authStateProvider).NotifyUserLoggedIn(result.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, result.Token);

                var returnValue = new LoginResponseModel() 
                { 
                    IsAuthenticated = true,
                    Token = result.Token,
                    User = result.User
                };

                return returnValue;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(Constants.LocalToken);
            await _localStorage.RemoveItemAsync(Constants.LocalUserDetails);

            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<UserDTO> GetCurrentLoggedInUser(int userId)
        {
            var loggedInUser = await _userService.GetUserRoleByIdAsync(userId);

            return loggedInUser;
        }

        //public async Task<GenericApiResponseModel> Forgot(string email)
        //{
        //    string requestUri = $"{_configuration.GetSection(Constants.eLoaApiUrl).Value}api/user/login/forgot?email={email}";

        //    using (var httpResponse = await _httpClient.PostAsync(requestUri, null))
        //    {
        //        var contentTemp = await httpResponse.Content.ReadAsStringAsync();
        //        var result = JsonConvert.DeserializeObject<GenericApiResponseModel>(contentTemp);

        //        return result;
        //    }
        //}

        //public async Task<string> GetEmailByResetCode(string resetCode)
        //{
        //    string requestUri = $"{_configuration.GetSection(Constants.eLoaApiUrl).Value}api/User/reset-code/{resetCode}";

        //    using (var httpResponse = await _httpClient.GetAsync(requestUri))
        //    {
        //        var contentTemp = await httpResponse.Content.ReadAsStringAsync();

        //        return contentTemp;
        //    }
        //}

        //public async Task<GenericApiResponseModel> Reset(ResetPasswordRequestModel requestModel)
        //{
        //    var content = JsonConvert.SerializeObject(requestModel);
        //    var bodyContent = new StringContent(content, Encoding.UTF8, Constants.MediaType);
        //    string requestUri = $"{_configuration.GetSection(Constants.eLoaApiUrl).Value}api/user/login/reset";

        //    using (var httpResponse = await _httpClient.PostAsync(requestUri, bodyContent))
        //    {
        //        var contentTemp = await httpResponse.Content.ReadAsStringAsync();
        //        var result = JsonConvert.DeserializeObject<GenericApiResponseModel>(contentTemp);

        //        return result;
        //    }
        //}
    }
}
