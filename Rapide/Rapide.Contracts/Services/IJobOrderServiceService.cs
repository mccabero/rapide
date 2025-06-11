using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IJobOrderServiceService : IBaseService<JobOrderService, JobOrderServiceDTO>
    {
        Task<JobOrderServiceDTO?> GetJobOrderServiceByIdAsync(int id);

        Task<List<JobOrderServiceDTO>> GetAllJobOrderServiceAsync();

        Task<List<JobOrderServiceDTO>> GetAllJobOrderServiceByJobOrderIdAsync(int jobOrderid);
    }
}
