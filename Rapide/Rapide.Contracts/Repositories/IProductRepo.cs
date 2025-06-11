using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IProductRepo : IBaseRepo<Product>
    {
        Task<Product?> GetProductByIdAsync(int id);

        Task<Product?> GetProductByPartNoAsync(string partNo);

        Task<List<Product>> GetAllProductAsync();
    }
}