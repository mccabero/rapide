using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class ServiceCategoryService(IServiceCategoryRepo repo) : BaseService<ServiceCategory, ServiceCategoryDTO>(repo), IServiceCategoryService
    {
        public async Task<List<ServiceCategoryDTO>> GetAllAsync()
        {
            try
            {
                List<ServiceCategoryDTO> dtoList = new List<ServiceCategoryDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                foreach (var e in entity)
                    dtoList.Add(e.Map<ServiceCategoryDTO>());
                
                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ServiceCategoryDTO?> GetAsync(Expression<Func<ServiceCategory, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<ServiceCategoryDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}