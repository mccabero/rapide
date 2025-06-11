using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IRoleService : IBaseService<Role, RoleDTO>
    {
        Task<List<RoleDTO>> GetAllAsync();
    }
}
