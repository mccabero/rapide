using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IInspectionTechnicianService : IBaseService<InspectionTechnician, InspectionTechnicianDTO>
    {
        Task<InspectionTechnicianDTO?> GetInspectionTechnicianByIdAsync(int id);

        Task<List<InspectionTechnicianDTO>> GetAllInspectionTechnicianAsync();

        Task<List<InspectionTechnicianDTO>> GetAllInspectionTechnicianByInspectionIdAsync(int inspectionId);
    }
}
