using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IServiceCategoryService : IBaseService<ServiceCategory, ServiceCategoryDTO>
    {
        Task<List<ServiceCategoryDTO>> GetAllAsync();
    }
}
