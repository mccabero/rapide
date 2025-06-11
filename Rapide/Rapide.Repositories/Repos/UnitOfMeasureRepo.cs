using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class UnitOfMeasureRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<UnitOfMeasure>(context), IUnitOfMeasureRepo
    {
        public async Task<List<UnitOfMeasure>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<UnitOfMeasure>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}