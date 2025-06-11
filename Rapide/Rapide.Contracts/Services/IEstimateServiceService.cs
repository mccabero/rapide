using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IEstimateServiceService : IBaseService<EstimateService, EstimateServiceDTO>
    {
        Task<EstimateServiceDTO?> GetEstimateServiceByIdAsync(int id);

        Task<List<EstimateServiceDTO>> GetAllEstimateServiceAsync();

        Task<List<EstimateServiceDTO>> GetAllEstimateServiceByEstimateIdAsync(int estimateId);
    }
}
