using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class ProductCategoryRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<ProductCategory>(context), IProductCategoryRepo
    {
        public async Task<List<ProductCategory>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<ProductCategory>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}