using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IJobStatusService : IBaseService<JobStatus, JobStatusDTO>
    {
        Task<List<JobStatusDTO>> GetAllAsync();
    }
}
