using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IVehicleModelRepo : IBaseRepo<VehicleModel>
    {
        Task<VehicleModel?> GetVehicleModelByIdAsync(int id);

        Task<List<VehicleModel>> GetAllVehicleModelAsync();
    }
}