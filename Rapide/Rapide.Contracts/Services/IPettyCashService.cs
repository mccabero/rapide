using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IPettyCashService : IBaseService<PettyCash, PettyCashDTO>
    {
        Task<PettyCashDTO?> GetPettyCashByIdAsync(int id);

        Task<List<PettyCashDTO>> GetAllPettyCashAsync();

        Task<PettyCashDetailsDTO?> GetPettyCashDetailsByIdAsync(int id);

        Task<List<PettyCashDetailsDTO>> GetAllPettyCashDetailsAsync();
    }
}
