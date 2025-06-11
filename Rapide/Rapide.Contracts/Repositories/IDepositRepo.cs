using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IDepositRepo : IBaseRepo<Deposit>
    {
        Task<Deposit?> GetDepositByIdAsync(int id);

        Task<List<Deposit>> GetAllDepositAsync();

        Task<List<Deposit>> GetAllDepositByCustomerIdAsync(int customerId);

        Task<List<Deposit>> GetAllDepositByJobOrderIdAsync(int jobOrderId);
    }
}