using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IPackageService : IBaseService<Package, PackageDTO>
    {
        Task<PackageDTO?> GetPackageByIdAsync(int id);

        Task<List<PackageDTO>> GetAllPackageAsync();
    }
}
