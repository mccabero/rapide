using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IPaymentService : IBaseService<Payment, PaymentDTO>
    {
        Task<PaymentDTO?> GetPaymentByIdAsync(int id);

        Task<List<PaymentDTO>> GetAllPaymentAsync();

        Task<List<PaymentDTO>> GetAllPaymentByCustomerIdAsync(int customerId);
    }
}
