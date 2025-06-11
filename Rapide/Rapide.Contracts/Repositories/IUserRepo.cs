using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IUserRepo : IBaseRepo<User>
    {
        Task<User?> GetUserRoleByIdAsync(int id);

        Task<User?> GetUserRoleByEmailAsync(string email);

        Task<List<User>> GetAllUserRoleAsync();
    }
}