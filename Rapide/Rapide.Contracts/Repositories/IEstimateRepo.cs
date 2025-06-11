using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IEstimateRepo : IBaseRepo<Estimate>
    {
        Task<Estimate?> GetEstimateByIdAsync(int id);

        Task<List<Estimate>> GetAllEstimateAsync();

        Task<List<Estimate>> GetAllEstimateByCustomerIdAsync(int customerId);
    }
}