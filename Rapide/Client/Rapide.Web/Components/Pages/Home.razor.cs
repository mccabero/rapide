using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Components.Pages.Customers;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Helpers;
using System.Threading.Tasks;

namespace Rapide.Web.Components.Pages
{
    public partial class Home
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [CascadingParameter]
        protected Task<AuthenticationState> AuthState { get; set; }

        [Inject]
        private IUserService UserServices { get; set; }
        [Inject]
        private ICustomerService CustomerServices { get; set; }
        #endregion

        #region Private Properties
        private bool isTechnician = false;

        private List<UserDTO> userList = new();
        private List<CustomerDTO> customerList = new();

        private List<UserDTO> EmployeeBirthdayMonthList = new();
        private List<CustomerDTO> CustomerBirthdayMonthList = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            isTechnician = TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.SeniorTechnician)
                || TokenHelper.IsRoleEqual(await AuthState, Constants.UserRoles.JuniorTechnician);

            userList = await UserServices.GetAllUserRoleAsync();
            customerList = await CustomerServices.GetAllAsync();

            var employeeBirthdayMonth = userList
                .Where(x => x.Birthday.HasValue && x.Birthday.Value.Month == DateTime.Now.Month && x.Birthday.Value.Day >= DateTime.Now.Day)
                .OrderBy(x => x.Birthday.Value.Day)
                .ToList();

            var customerBirhtdayMonth = customerList
                .Where(x => x.Birthday.HasValue && x.Birthday.Value.Month == DateTime.Now.Month && x.Birthday.Value.Day >= DateTime.Now.Day)
                .OrderBy(x => x.Birthday.Value.Day)
                .ToList();

            foreach (var ebm in employeeBirthdayMonth)
            {
                if (!EmployeeBirthdayMonthList.Any(x => x.Id == ebm.Id))
                {
                    EmployeeBirthdayMonthList.Add(ebm);
                }
            }

            foreach (var cbm in customerBirhtdayMonth)
            {
                if (!CustomerBirthdayMonthList.Any(x => x.Id == cbm.Id))
                {
                    CustomerBirthdayMonthList.Add(cbm);
                }
            }

            await base.OnInitializedAsync();
        }

        
    }
}
