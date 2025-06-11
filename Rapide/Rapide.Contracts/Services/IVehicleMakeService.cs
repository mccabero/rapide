using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IVehicleMakeService : IBaseService<VehicleMake, VehicleMakeDTO>
    {
        Task<List<VehicleMakeDTO>> GetAllAsync();

        Task<VehicleMakeDTO?> GetVehicleMakeByIdAsync(int id);
    }
}
