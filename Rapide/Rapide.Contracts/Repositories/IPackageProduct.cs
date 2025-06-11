using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IPackageProductRepo : IBaseRepo<PackageProduct>
    {
        Task<PackageProduct?> GetPackageProductByIdAsync(int id);

        Task<List<PackageProduct>> GetAllPackageProductAsync();

        Task<List<PackageProduct>> GetAllPackageProductByPackageIdAsync(int packageId);
    }
}