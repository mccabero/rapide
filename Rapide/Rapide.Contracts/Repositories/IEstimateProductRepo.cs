using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IEstimateProductRepo : IBaseRepo<EstimateProduct>
    {
        Task<EstimateProduct?> GetEstimateProductByIdAsync(int id);

        Task<List<EstimateProduct>> GetAllEstimateProductAsync();

        Task<List<EstimateProduct>> GetAllEstimateProductByEstimateIdAsync(int estimateId);
    }
}