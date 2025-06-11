using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface ISupplierRepo : IBaseRepo<Supplier>
    {
        Task<List<Supplier>> GetAllAsync();
    }
}