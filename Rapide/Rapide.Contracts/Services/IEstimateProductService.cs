using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IEstimateProductService : IBaseService<EstimateProduct, EstimateProductDTO>
    {
        Task<EstimateProductDTO?> GetEstimateProductByIdAsync(int id);

        Task<List<EstimateProductDTO>> GetAllEstimateProductAsync();

        Task<List<EstimateProductDTO>> GetAllEstimateProductByEstimateIdAsync(int estimateId);
    }
}
