using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IInspectionTechnicianRepo : IBaseRepo<InspectionTechnician>
    {
        Task<InspectionTechnician?> GetInspectionTechnicianByIdAsync(int id);

        Task<List<InspectionTechnician>> GetAllInspectionTechnicianAsync();

        Task<List<InspectionTechnician>> GetAllInspectionTechnicianByInspectionIdAsync(int inspectionId);
    }
}