using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IPackageServiceRepo : IBaseRepo<PackageService>
    {
        Task<PackageService?> GetPackageServiceByIdAsync(int id);

        Task<List<PackageService>> GetAllPackageServiceAsync();

        Task<List<PackageService>> GetAllPackageServiceByPackageIdAsync(int packageId);
    }
}