using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IManufacturerService : IBaseService<Manufacturer, ManufacturerDTO>
    {
        Task<List<ManufacturerDTO>> GetAllAsync();
    }
}
