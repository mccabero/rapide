using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IJobOrderServiceRepo : IBaseRepo<JobOrderService>
    {
        Task<JobOrderService?> GetJobOrderServiceByIdAsync(int id);

        Task<List<JobOrderService>> GetAllJobOrderServiceAsync();

        Task<List<JobOrderService>> GetAllJobOrderServiceByJobOrderIdAsync(int jobOrderBy);
    }
}