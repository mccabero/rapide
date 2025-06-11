using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IParameterService : IBaseService<Parameter, ParameterDTO>
    {
        Task<ParameterDTO?> GetParameterByIdAsync(int id);

        Task<List<ParameterDTO>> GetAllParameterAsync();
    }
}
