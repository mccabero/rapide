using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IInvoiceService : IBaseService<Invoice, InvoiceDTO>
    {
        Task<InvoiceDTO?> GetInvoiceByIdAsync(int id);

        Task<List<InvoiceDTO>> GetAllInvoiceAsync();

        Task<List<InvoiceDTO>> GetAllInvoiceByCustomerIdAsync(int customerId);
    }
}
