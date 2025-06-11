using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IProductCategoryRepo : IBaseRepo<ProductCategory>
    {
        Task<List<ProductCategory>> GetAllAsync();
    }
}