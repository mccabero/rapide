using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IServiceGroupService : IBaseService<ServiceGroup, ServiceGroupDTO>
    {
        Task<List<ServiceGroupDTO>> GetAllAsync();
    }
}
