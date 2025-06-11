using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IJobOrderTechnicianRepo : IBaseRepo<JobOrderTechnician>
    {
        Task<JobOrderTechnician?> GetJobOrderTechnicianByIdAsync(int id);

        Task<List<JobOrderTechnician>> GetAllJobOrderTechnicianAsync();

        Task<List<JobOrderTechnician>> GetAllJobOrderTechnicianByJobOrderIdAsync(int estimateId);
    }
}