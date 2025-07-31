using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IInspectionRepo : IBaseRepo<Inspection>
    {
        Task<Inspection?> GetInspectionByIdAsync(int id);

        Task<List<Inspection>> GetAllInspectionAsync();

        Task<List<Inspection>> GetAllInspectionSummaryAsync();

        Task<List<Inspection>> GetAllInspectionByCustomerIdAsync(int customerId);

        Task<List<Inspection>> GetAllInspectionByVehicleIdAsync(int vehicleId);
    }
}