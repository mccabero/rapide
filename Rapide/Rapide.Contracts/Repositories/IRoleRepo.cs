using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IRoleRepo : IBaseRepo<Role>
    {
        Task<List<Role>> GetAllAsync();
    }
}