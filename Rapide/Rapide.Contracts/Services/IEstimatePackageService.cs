using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IEstimatePackageService : IBaseService<EstimatePackage, EstimatePackageDTO>
    {
        Task<EstimatePackageDTO?> GetEstimatePackageByIdAsync(int id);

        Task<List<EstimatePackageDTO>> GetAllEstimatePackageAsync();

        Task<List<EstimatePackageDTO>> GetAllEstimatePackageByEstimateIdAsync(int estimateId);
    }
}
