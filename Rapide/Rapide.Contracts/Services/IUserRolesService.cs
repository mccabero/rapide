using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IUserRolesService : IBaseService<UserRoles, UserRolesDTO>
    {
        Task<List<UserRolesDTO>> GetAllUserRolesAsync();

        Task<List<UserRolesDTO>> GetUserRolesByUserIdAsync(int userId);
    }
}
