using Microsoft.AspNetCore.Components;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Helpers;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class JobOrderTechnicianComponent
    {
        #region Parameters
        [Parameter]
        public List<JobOrderTechnicianDTO> JobOrderTechnicianParam { get; set; }
        [Parameter]
        public bool IsJobOrderLocked { get; set; }
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
            IsTechnicianLimitExceeded = JobOrderTechnicianParam.Count <= 4;

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private void RemoveValue(JobOrderTechnicianDTO i) 
            => JobOrderTechnicianParam.Remove(i);
        
        private void OnAddTechnicianClick()
        {
            if (JobOrderTechnicianParam.Count == 4)
            {
                IsTechnicianLimitExceeded = true;
                
                StateHasChanged();
                return;
            }

            foreach (var u in JobOrderTechnicianParam)
                UserList.Remove(u.TechnicianUser);

            JobOrderTechnicianParam.Add(new JobOrderTechnicianDTO()
            {
                JobOrder = new JobOrderDTO(),
                TechnicianUser = new UserDTO()
                {
                    Role = new RoleDTO()
                }
            }); ;

            StateHasChanged();
        }

        private void OnAddNewItemClick()
        {
            var jobOrderId = JobOrderTechnicianParam.FirstOrDefault().JobOrderId;
            bool isEditMode = jobOrderId > 0;
            var returnUrl = isEditMode
                ? $"/operations/job-orders/{jobOrderId}"
                : "/operations/job-orders/add";

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