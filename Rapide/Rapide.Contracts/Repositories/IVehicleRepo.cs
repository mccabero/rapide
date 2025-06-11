using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IVehicleRepo : IBaseRepo<Vehicle>
    {
        Task<Vehicle?> GetVehicleByIdAsync(int id);

        Task<Vehicle?> GetVehicleByModelAsync(string model);

        Task<List<Vehicle>> GetAllVehicleAsync();

        Task<List<Vehicle>> GetAllVehicleByCustomerIdAsync(int customerId);
    }
}