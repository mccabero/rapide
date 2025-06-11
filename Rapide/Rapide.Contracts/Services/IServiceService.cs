using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IServiceService : IBaseService<Service, ServiceDTO>
    {
        Task<ServiceDTO?> GetServiceByIdAsync(int id);

        Task<ServiceDTO?> GetServiceByCodeAsync(string code);

        Task<List<ServiceDTO>> GetAllServiceAsync();
    }
}
