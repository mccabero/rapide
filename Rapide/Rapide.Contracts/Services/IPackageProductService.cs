using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IPackageProductService : IBaseService<PackageProduct, PackageProductDTO>
    {
        Task<PackageProductDTO?> GetPackageProductByIdAsync(int id);

        Task<List<PackageProductDTO>> GetAllPackageProductAsync();

        Task<List<PackageProductDTO>> GetAllPackageProductByPackageIdAsync(int packageId);
    }
}
