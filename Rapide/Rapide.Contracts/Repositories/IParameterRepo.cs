using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IParameterRepo : IBaseRepo<Parameter>
    {
        Task<Parameter?> GetParameterByIdAsync(int id);

        Task<List<Parameter>> GetAllParameterAsync();
    }
}