using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IInvoiceRepo : IBaseRepo<Invoice>
    {
        Task<Invoice?> GetInvoiceByIdAsync(int id);

        Task<List<Invoice>> GetAllInvoiceAsync();

        Task<List<Invoice>> GetAllInvoiceByCustomerIdAsync(int customerId);
    }
}