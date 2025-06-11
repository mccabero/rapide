using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IPackageRepo : IBaseRepo<Package>
    {
        Task<Package?> GetPackageByIdAsync(int id);

        Task<List<Package>> GetAllPackageAsync();
    }
}