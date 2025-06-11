using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IJobOrderProductRepo : IBaseRepo<JobOrderProduct>
    {
        Task<JobOrderProduct?> GetJobOrderProductByIdAsync(int id);

        Task<List<JobOrderProduct>> GetAllJobOrderProductAsync();

        Task<List<JobOrderProduct>> GetAllJobOrderProductByJobOrderIdAsync(int jobOrderId);
    }
}