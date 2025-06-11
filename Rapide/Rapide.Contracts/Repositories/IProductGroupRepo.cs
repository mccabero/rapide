using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IProductGroupRepo : IBaseRepo<ProductGroup>
    {
        Task<List<ProductGroup>> GetAllAsync();
    }
}