using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IServiceGroupRepo : IBaseRepo<ServiceGroup>
    {
        Task<List<ServiceGroup>> GetAllAsync();
    }
}