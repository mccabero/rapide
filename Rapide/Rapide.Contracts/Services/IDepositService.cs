using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IDepositService : IBaseService<Deposit, DepositDTO>
    {
        Task<DepositDTO?> GetDepositByIdAsync(int id);

        Task<List<DepositDTO>> GetAllDepositAsync();

        Task<List<DepositDTO>> GetAllDepositByCustomerIdAsync(int customerId);

        Task<List<DepositDTO>> GetAllDepositByJobOrderIdAsync(int jobOrderId);
    }
}
