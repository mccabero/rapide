using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class ServiceCategoryRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<ServiceCategory>(context), IServiceCategoryRepo
    {
        public async Task<List<ServiceCategory>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<ServiceCategory>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}