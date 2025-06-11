using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IJobStatusRepo : IBaseRepo<JobStatus>
    {
        Task<List<JobStatus>> GetAllAsync();
    }
}