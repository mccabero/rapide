using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IVehicleModelService : IBaseService<VehicleModel, VehicleModelDTO>
    {
        Task<VehicleModelDTO?> GetVehicleModelByIdAsync(int id);

        Task<List<VehicleModelDTO>> GetAllVehicleModelAsync();
    }
}
