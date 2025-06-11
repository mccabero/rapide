using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IJobOrderPackageService : IBaseService<JobOrderPackage, JobOrderPackageDTO>
    {
        Task<JobOrderPackageDTO?> GetJobOrderPackageByIdAsync(int id);

        Task<List<JobOrderPackageDTO>> GetAllJobOrderPackageAsync();

        Task<List<JobOrderPackageDTO>> GetAllJobOrderPackageByJobOrderIdAsync(int jobOrderId);
    }
}
