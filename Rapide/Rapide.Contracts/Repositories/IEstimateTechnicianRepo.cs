using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IEstimateTechnicianRepo : IBaseRepo<EstimateTechnician>
    {
        Task<EstimateTechnician?> GetEstimateTechnicianByIdAsync(int id);

        Task<List<EstimateTechnician>> GetAllEstimateTechnicianAsync();

        Task<List<EstimateTechnician>> GetAllEstimateTechnicianByEstimateIdAsync(int estimateId);
    }
}