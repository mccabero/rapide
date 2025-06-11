using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IEstimateServiceRepo : IBaseRepo<EstimateService>
    {
        Task<EstimateService?> GetEstimateServiceByIdAsync(int id);

        Task<List<EstimateService>> GetAllEstimateServiceAsync();

        Task<List<EstimateService>> GetAllEstimateServiceByEstimateIdAsync(int estimateId);
    }
}