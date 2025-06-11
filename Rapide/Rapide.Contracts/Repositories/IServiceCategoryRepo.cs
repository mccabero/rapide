using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IServiceCategoryRepo : IBaseRepo<ServiceCategory>
    {
        Task<List<ServiceCategory>> GetAllAsync();
    }
}