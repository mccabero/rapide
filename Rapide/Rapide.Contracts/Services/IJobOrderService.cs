using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IJobOrderService : IBaseService<JobOrder, JobOrderDTO>
    {
        Task<JobOrderDTO?> GetJobOrderByIdAsync(int id);

        Task<List<JobOrderDTO>> GetAllJobOrderAsync();

        Task<List<JobOrderDTO>> GetAllJobOrderByCustomerIdAsync(int customerId);

        Task<List<JobOrderDTO>> GetAllJobOrderByVehicleIdAsync(int vehicleId);
    }
}
