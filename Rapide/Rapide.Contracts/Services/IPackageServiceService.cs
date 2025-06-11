using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IPackageServiceService : IBaseService<PackageService, PackageServiceDTO>
    {
        Task<PackageServiceDTO?> GetPackageServiceByIdAsync(int id);

        Task<List<PackageServiceDTO>> GetAllPackageServiceAsync();

        Task<List<PackageServiceDTO>> GetAllPackageServiceByPackageIdAsync(int packageId);
    }
}
