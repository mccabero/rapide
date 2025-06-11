using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IEstimatePackageRepo : IBaseRepo<EstimatePackage>
    {
        Task<EstimatePackage?> GetEstimatePackageByIdAsync(int id);

        Task<List<EstimatePackage>> GetAllEstimatePackageAsync();

        Task<List<EstimatePackage>> GetAllEstimatePackageByEstimateIdAsync(int estimateId);
    }
}