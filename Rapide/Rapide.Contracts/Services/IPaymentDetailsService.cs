using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IPaymentDetailsService : IBaseService<PaymentDetails, PaymentDetailsDTO>
    {
        Task<PaymentDetailsDTO?> GetPaymentDetailsByIdAsync(int id);

        Task<List<PaymentDetailsDTO>> GetAllPaymentDetailsAsync();

        Task<List<PaymentDetailsDTO>> GetAllPaymentDetailsByPaymentIdAsync(int paymentId);

        Task<List<PaymentDetailsDTO>> GetAllPaymentDetailsByInvoiceIdAsync(int invoiceId);
    }
}
