using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IParameterGroupService : IBaseService<ParameterGroup, ParameterGroupDTO>
    {
        Task<List<ParameterGroupDTO>> GetAllAsync();
    }
}
