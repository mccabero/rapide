using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IVehicleMakeRepo : IBaseRepo<VehicleMake>
    {
        Task<List<VehicleMake?>> GetAllAsync();

        Task<VehicleMake> GetVehicleMakeByIdAsync(int id);
    }
}