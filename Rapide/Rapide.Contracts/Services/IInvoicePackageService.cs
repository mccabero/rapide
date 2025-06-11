using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IInvoicePackageService : IBaseService<InvoicePackage, InvoicePackageDTO>
    {
        Task<InvoicePackageDTO?> GetInvoicePackageByIdAsync(int id);

        Task<List<InvoicePackageDTO>> GetAllInvoicePackageAsync();

        Task<List<InvoicePackageDTO>> GetAllInvoicePackageByInvoiceIdAsync(int invoiceId);
    }
}
