using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class UnitOfMeasureService(IUnitOfMeasureRepo repo) : BaseService<UnitOfMeasure, UnitOfMeasureDTO>(repo), IUnitOfMeasureService
    {
        public async Task<List<UnitOfMeasureDTO>> GetAllAsync()
        {
            try
            {
                List<UnitOfMeasureDTO> dtoList = new List<UnitOfMeasureDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                foreach (var e in entity)
                    dtoList.Add(e.Map<UnitOfMeasureDTO>());
                
                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public override async Task<UnitOfMeasureDTO?> GetAsync(Expression<Func<UnitOfMeasure, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<UnitOfMeasureDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
