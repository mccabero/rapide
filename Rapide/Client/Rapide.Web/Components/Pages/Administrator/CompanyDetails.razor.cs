using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Services;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Administrator
{
    public partial class CompanyDetails
    {
        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }
        [Inject]
        private ICompanyInfoService CompanyInfoService { get; set; }
        [Inject]
        private ISnackbar SnackbarService { get; set; }
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IWebHostEnvironment _environment { get; set; }
        #endregion

        #region Private Properties
        private MudForm form;
        private string[] errors = { };
        private bool success;

        private MudMessageBox mboxCustom { get; set; }
        private string mBoxCustomMessage { get; set; }
        private MudMessageBox mboxError { get; set; }
        private MudMessageBox mbox { get; set; }
        private bool IsLoading { get; set; }
        private bool IsEditMode { get; set; }
        private string imageSource = string.Empty;

        private List<string> companyLogo = new List<string>();
        private List<string> defaultlogo = new List<string>();

        private CompanyInfoDTO CompanyRequestModel { get; set; } = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            CompanyRequestModel = new CompanyInfoDTO();

            IsEditMode = false;
            var companyInfo = await CompanyInfoService.GetAllAsync();

            if (companyInfo != null && companyInfo.Any())
            {
                IsEditMode = true;
                CompanyRequestModel = companyInfo.FirstOrDefault();
            }

            if (IsEditMode)
            {
                GetApplicationLogo();
            }

            await base.OnInitializedAsync();
        }

        private void OpenFileDefault(string filename, bool isDefault)
        {
            var folderLocation = isDefault
                ? "default-logo"
                : "company-logo";

            imageSource = $"/images/{folderLocation}/{Path.GetFileName(filename)}";
            StateHasChanged();
        }

        private void GetApplicationLogo()
        {
            var filesDirectory = Path.Combine(_environment.WebRootPath, "images", "company-logo");
            var filesDirectoryDefault = Path.Combine(_environment.WebRootPath, "images", "default-logo");


            string[] fileEntries = Directory.GetFiles(filesDirectory);
            string[] fileEntriesDefault = Directory.GetFiles(filesDirectoryDefault);

            companyLogo = new List<string>();
            defaultlogo = new List<string>();

            foreach (string fileName in fileEntries)
                companyLogo.Add(fileName);
            
            foreach (string fileName in fileEntriesDefault)
                defaultlogo.Add(fileName);

        }

        IList<IBrowserFile> _files = new List<IBrowserFile>();
        private async Task UploadFiles(InputFileChangeEventArgs args)
        {
            try
            {
                _files.Add(args.File);

                var uploadDirectory = Path.Combine(_environment.WebRootPath, "images", "company-logo");

                if (!Directory.Exists(uploadDirectory))
                    Directory.CreateDirectory(uploadDirectory);

                string[] fileEntries = Directory.GetFiles(uploadDirectory);
                foreach (string fileName in fileEntries)
                    File.Delete(fileName);

                //var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var fileExtension = new FileInfo(args.File.Name);
                var path = Path.Combine(uploadDirectory, $"company-logo{fileExtension.Extension}");


                await using var fs = new FileStream(path, FileMode.Create);
                await args.File.OpenReadStream(9512000).CopyToAsync(fs);

                GetApplicationLogo();
                StateHasChanged();
                NavigationManager.NavigateToCustom("/administrators/company", true);
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

        private async Task UploadFilesDefault(InputFileChangeEventArgs args)
        {
            try
            {
                _files.Add(args.File);

                var uploadDirectory = Path.Combine(_environment.WebRootPath, "images", "default-logo");

                if (!Directory.Exists(uploadDirectory))
                    Directory.CreateDirectory(uploadDirectory);

                string[] fileEntries = Directory.GetFiles(uploadDirectory);
                foreach (string fileName in fileEntries)
                    File.Delete(fileName);

                //var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var fileExtension = new FileInfo(args.File.Name);
                var path = Path.Combine(uploadDirectory, $"default-logo{fileExtension.Extension}");


                await using var fs = new FileStream(path, FileMode.Create);
                await args.File.OpenReadStream(9512000).CopyToAsync(fs);

                GetApplicationLogo();
                StateHasChanged();
                NavigationManager.NavigateToCustom("/administrators/company", true);
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
                    if (!IsEditMode)
                    {
                        // create mode
                        CompanyRequestModel.CreatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        CompanyRequestModel.CreatedDateTime = DateTime.Now;
                        CompanyRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        CompanyRequestModel.UpdatedDateTime = DateTime.Now;

                        // call create endpoint here...
                        var created = await CompanyInfoService.CreateAsync(CompanyRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Company Info Successfuly Added!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/", true);
                    }
                    else // update mode
                    {
                        CompanyRequestModel.UpdatedById = TokenHelper.GetCurrentUserId(await AuthState);
                        CompanyRequestModel.UpdatedDateTime = DateTime.Now;

                        // call update endpoint here...
                        await CompanyInfoService.UpdateAsync(CompanyRequestModel);

                        IsLoading = false;
                        StateHasChanged();

                        SnackbarService.Add("Company Info Successfuly Updated!", Severity.Normal, config => { config.ShowCloseIcon = true; });
                        NavigationManager.NavigateToCustom("/", true);
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
            NavigationManager.NavigateToCustom("/", true);
        }
    }
}
