using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IServiceRepo : IBaseRepo<Service>
    {
        Task<Service?> GetServiceByIdAsync(int id);

        Task<Service?> GetServiceByCodeAsync(string code);

        Task<List<Service>> GetAllServiceAsync();
    }
}