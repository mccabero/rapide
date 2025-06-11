using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IInspectionService : IBaseService<Inspection, InspectionDTO>
    {
        Task<InspectionDTO?> GetInspectionByIdAsync(int id);

        Task<List<InspectionDTO>> GetAllInspectionAsync();

        Task<List<InspectionDTO>> GetAllInspectionByCustomerIdAsync(int customerId);

        Task<List<InspectionDTO>> GetAllInspectionByVehicleIdAsync(int vehicleId);
    }
}
