using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class ProductGroupService(IProductGroupRepo repo) : BaseService<ProductGroup, ProductGroupDTO>(repo), IProductGroupService
    {
        public async Task<List<ProductGroupDTO>> GetAllAsync()
        {
            try
            {
                List<ProductGroupDTO> dtoList = new List<ProductGroupDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                foreach (var e in entity)
                    dtoList.Add(e.Map<ProductGroupDTO>());
                
                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ProductGroupDTO?> GetAsync(Expression<Func<ProductGroup, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<ProductGroupDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}