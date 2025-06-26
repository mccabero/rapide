using Microsoft.AspNetCore.Components;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class InspectionTechnicianComponent
    {
        #region Parameters
        [Parameter]
        public List<InspectionTechnicianDTO> InspectionTechnicianParam { get; set; }
        [Parameter]
        public bool IsInspectionLocked { get; set; }
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
            IsTechnicianLimitExceeded = InspectionTechnicianParam.Count <= 4;

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private void RemoveValue(InspectionTechnicianDTO i) 
            => InspectionTechnicianParam.Remove(i);
        
        private void OnAddTechnicianClick()
        {
            if (InspectionTechnicianParam.Count == 10)
            {
                IsTechnicianLimitExceeded = true;
                
                StateHasChanged();
                return;
            }

            foreach (var u in InspectionTechnicianParam)
                UserList.Remove(u.TechnicianUser);

            InspectionTechnicianParam.Add(new InspectionTechnicianDTO()
            {
                Inspection = new InspectionDTO(),
                TechnicianUser = new UserDTO()
                {
                    Role = new RoleDTO()
                }
            }); ;

            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            var inspectionId = InspectionTechnicianParam.FirstOrDefault().InspectionId;
            bool isEditMode = inspectionId > 0;
            var returnUrl = isEditMode
                ? $"/operations/inspections/{inspectionId}"
                : "/operations/inspections/add";

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