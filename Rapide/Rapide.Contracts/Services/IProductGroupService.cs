using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IProductGroupService : IBaseService<ProductGroup, ProductGroupDTO>
    {
        Task<List<ProductGroupDTO>> GetAllAsync();
    }
}
