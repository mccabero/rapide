using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IJobOrderProductService : IBaseService<JobOrderProduct, JobOrderProductDTO>
    {
        Task<JobOrderProductDTO?> GetJobOrderProductByIdAsync(int id);

        Task<List<JobOrderProductDTO>> GetAllJobOrderProductAsync();

        Task<List<JobOrderProductDTO>> GetAllJobOrderProductByJobOrderIdAsync(int jobOrderId);
    }
}
