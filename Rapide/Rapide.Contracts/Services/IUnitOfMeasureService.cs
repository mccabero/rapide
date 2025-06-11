using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IUnitOfMeasureService : IBaseService<UnitOfMeasure, UnitOfMeasureDTO>
    {
        Task<List<UnitOfMeasureDTO>> GetAllAsync();
    }
}
