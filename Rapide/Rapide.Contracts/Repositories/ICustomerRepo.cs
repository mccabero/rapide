using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface ICustomerRepo : IBaseRepo<Customer>
    {
        Task<List<Customer>> GetAllAsync();
    }
}