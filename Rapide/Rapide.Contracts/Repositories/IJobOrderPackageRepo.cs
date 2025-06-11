using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IJobOrderPackageRepo : IBaseRepo<JobOrderPackage>
    {
        Task<JobOrderPackage?> GetJobOrderPackageByIdAsync(int id);

        Task<List<JobOrderPackage>> GetAllJobOrderPackageAsync();

        Task<List<JobOrderPackage>> GetAllJobOrderPackageByJobOrderIdAsync(int jobOrderId);
    }
}