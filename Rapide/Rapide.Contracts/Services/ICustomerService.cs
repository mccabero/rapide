using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface ICustomerService : IBaseService<Customer, CustomerDTO>
    {
        Task<List<CustomerDTO>> GetAllAsync();
    }
}
