using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IPaymentDetailsRepo : IBaseRepo<PaymentDetails>
    {
        Task<PaymentDetails?> GetPaymentDetailsByIdAsync(int id);

        Task<List<PaymentDetails>> GetAllPaymentDetailsAsync();

        Task<List<PaymentDetails>> GetAllPaymentDetailsByPaymentIdAsync(int paymentId);

        Task<List<PaymentDetails>> GetAllPaymentDetailsByInvoiceIdAsync(int invoiceId);
    }
}