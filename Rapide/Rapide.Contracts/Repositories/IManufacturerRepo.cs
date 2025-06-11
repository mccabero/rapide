using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IManufacturerRepo : IBaseRepo<Manufacturer>
    {
        Task<List<Manufacturer>> GetAllAsync();
    }
}