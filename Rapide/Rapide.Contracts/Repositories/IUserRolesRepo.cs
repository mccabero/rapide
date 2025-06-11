using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IUserRolesRepo : IBaseRepo<UserRoles>
    {
        Task<List<UserRoles>> GetAllUserRolesAsync();

        Task<List<UserRoles>> GetUserRolesByUserIdAsync(int userId);
    }
}