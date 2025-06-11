using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class ManufacturerService(IManufacturerRepo repo) : BaseService<Manufacturer, ManufacturerDTO>(repo), IManufacturerService
    {
        public async Task<List<ManufacturerDTO>> GetAllAsync()
        {
            try
            {
                List<ManufacturerDTO> dtoList = new List<ManufacturerDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                foreach (var e in entity)
                    dtoList.Add(e.Map<ManufacturerDTO>());
                
                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ManufacturerDTO?> GetAsync(Expression<Func<Manufacturer, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<ManufacturerDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}