using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface ISupplierService : IBaseService<Supplier, SupplierDTO>
    {
        Task<List<SupplierDTO>> GetAllAsync();
    }
}
