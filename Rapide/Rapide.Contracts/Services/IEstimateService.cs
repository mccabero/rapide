using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IEstimateService : IBaseService<Estimate, EstimateDTO>
    {
        Task<EstimateDTO?> GetEstimateByIdAsync(int id);

        Task<List<EstimateDTO>> GetAllEstimateAsync();

        Task<List<EstimateDTO>> GetAllEstimateByCustomerIdAsync(int customerId);
    }
}
