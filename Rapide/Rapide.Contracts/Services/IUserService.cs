using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IUserService : IBaseService<User, UserDTO>
    {
        Task<UserDTO?> GetUserRoleByIdAsync(int id);

        Task<UserDTO?> GetUserRoleByEmailAsync(string email);

        Task<List<UserDTO>> GetAllUserRoleAsync();
    }
}
