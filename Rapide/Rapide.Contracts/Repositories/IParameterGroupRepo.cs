using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IParameterGroupRepo : IBaseRepo<ParameterGroup>
    {
        Task<List<ParameterGroup>> GetAllAsync();
    }
}