using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IUnitOfMeasureRepo : IBaseRepo<UnitOfMeasure>
    {
        Task<List<UnitOfMeasure>> GetAllAsync();
    }
}