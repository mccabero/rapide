using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class ServiceGroupRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<ServiceGroup>(context), IServiceGroupRepo
    {
        public async Task<List<ServiceGroup>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<ServiceGroup>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}