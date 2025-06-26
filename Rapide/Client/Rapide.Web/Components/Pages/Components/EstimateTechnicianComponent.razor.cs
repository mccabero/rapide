using Microsoft.AspNetCore.Components;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class EstimateTechnicianComponent
    {
        #region Parameters
        [Parameter]
        public List<EstimateTechnicianDTO> EstimateTechnicianParam { get; set; }
        [Parameter]
        public bool IsEstimateLocked { get; set; }
        #endregion

        #region Dependency Injection
        [Inject]
        private IUserService UserService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private List<UserDTO> UserList { get; set; } = new();
        private bool IsTechnicianLimitExceeded { get; set; } = false;
        #endregion
        
        protected override async Task OnInitializedAsync()
        {
            var users = await UserService.GetAllUserRoleAsync();
            UserList = users.Where(x => x.Role.Name.ToUpper().Contains("TECHNICIAN") && x.IsActive == true).ToList();
            IsTechnicianLimitExceeded = EstimateTechnicianParam.Count <= 10;

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private void RemoveValue(EstimateTechnicianDTO i) 
            => EstimateTechnicianParam.Remove(i);
        
        private void OnAddTechnicianClick()
        {
            if (EstimateTechnicianParam.Count == 10)
            {
                IsTechnicianLimitExceeded = true;
                
                StateHasChanged();
                return;
            }

            foreach (var u in EstimateTechnicianParam)
                UserList.Remove(u.TechnicianUser);

            EstimateTechnicianParam.Add(new EstimateTechnicianDTO()
            {
                Estimate = new EstimateDTO(),
                TechnicianUser = new UserDTO()
                {
                    Role = new RoleDTO()
                }
            }); ;

            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            var estimateId = EstimateTechnicianParam.FirstOrDefault().EstimateId;
            bool isEditMode = estimateId > 0;
            var returnUrl = isEditMode
                ? $"/operations/estimates/{estimateId}"
                : "/operations/estimates/add";

            NavigationManager.NavigateToCustom($"/administrators/users/add?returnUrl={returnUrl}");
        }

        private async Task<IEnumerable<UserDTO>> SearchTechnician(string filter, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filter))
                return UserList;

            return UserList.Where(i => $"{i.FirstName} {i.LastName}".Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}