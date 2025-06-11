using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IVehicleService : IBaseService<Vehicle, VehicleDTO>
    {
        Task<VehicleDTO?> GetVehicleByIdAsync(int id);

        Task<VehicleDTO?> GetVehicleByModelAsync(string model);

        Task<List<VehicleDTO>> GetAllVehicleAsync();

        Task<List<VehicleDTO>> GetAllVehicleByCustomerIdAsync(int customerId);
    }
}
