using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IInvoicePackageRepo : IBaseRepo<InvoicePackage>
    {
        Task<InvoicePackage?> GetInvoicePackageByIdAsync(int id);

        Task<List<InvoicePackage>> GetAllInvoicePackageAsync();

        Task<List<InvoicePackage>> GetAllInvoicePackageByInvoiceIdAsync(int invoiceId);
    }
}