using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IProductService : IBaseService<Product, ProductDTO>
    {
        Task<ProductDTO?> GetProductByIdAsync(int id);

        Task<ProductDTO?> GetProductByPartNoAsync(string partNo);

        Task<List<ProductDTO>> GetAllProductAsync();
    }
}
