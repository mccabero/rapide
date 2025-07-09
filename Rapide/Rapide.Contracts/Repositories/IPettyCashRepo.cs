using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IPettyCashRepo : IBaseRepo<PettyCash>
    {
        Task<PettyCash?> GetPettyCashByIdAsync(int id);

        Task<List<PettyCash>> GetAllPettyCashAsync();

        Task<PettyCashDetails?> GetPettyCashDetailsByIdAsync(int id);

        Task<List<PettyCashDetails>> GetAllPettyCashDetailsAsync();
    }
}