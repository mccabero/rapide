using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IPaymentRepo : IBaseRepo<Payment>
    {
        Task<Payment?> GetPaymentByIdAsync(int id);

        Task<List<Payment>> GetAllPaymentAsync();

        Task<List<Payment>> GetAllPaymentByCustomerIdAsync(int customerId);
    }
}