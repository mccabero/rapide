using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IProductCategoryService : IBaseService<ProductCategory, ProductCategoryDTO>
    {
        Task<List<ProductCategoryDTO>> GetAllAsync();
    }
}
