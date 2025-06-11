using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IJobOrderRepo : IBaseRepo<JobOrder>
    {
        Task<JobOrder?> GetJobOrderByIdAsync(int id);

        Task<List<JobOrder>> GetAllJobOrderAsync();

        Task<List<JobOrder>> GetAllJobOrderByCustomerIdAsync(int customerId);

        Task<List<JobOrder>> GetAllJobOrderByVehicleIdAsync(int vehicleId);
    }
}