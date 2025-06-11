using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IJobOrderTechnicianService : IBaseService<JobOrderTechnician, JobOrderTechnicianDTO>
    {
        Task<JobOrderTechnicianDTO?> GetJobOrderTechnicianByIdAsync(int id);

        Task<List<JobOrderTechnicianDTO>> GetAllJobOrderTechnicianAsync();

        Task<List<JobOrderTechnicianDTO>> GetAllJobOrderTechnicianByJobOrderIdAsync(int estimateId);
    }
}
