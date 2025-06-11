using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class ProductCategoryService(IProductCategoryRepo repo) : BaseService<ProductCategory, ProductCategoryDTO>(repo), IProductCategoryService
    {
        public async Task<List<ProductCategoryDTO>> GetAllAsync()
        {
            try
            {
                List<ProductCategoryDTO> dtoList = new List<ProductCategoryDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                foreach (var e in entity)
                    dtoList.Add(e.Map<ProductCategoryDTO>());
                
                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ProductCategoryDTO?> GetAsync(Expression<Func<ProductCategory, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<ProductCategoryDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}