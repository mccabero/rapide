using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IEstimateTechnicianService : IBaseService<EstimateTechnician, EstimateTechnicianDTO>
    {
        Task<EstimateTechnicianDTO?> GetEstimateTechnicianByIdAsync(int id);

        Task<List<EstimateTechnicianDTO>> GetAllEstimateTechnicianAsync();

        Task<List<EstimateTechnicianDTO>> GetAllEstimateTechnicianByEstimateIdAsync(int estimateId);
    }
}
