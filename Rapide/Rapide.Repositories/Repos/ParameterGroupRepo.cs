using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class ParameterGroupRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<ParameterGroup>(context), IParameterGroupRepo
    {
        public async Task<List<ParameterGroup>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<ParameterGroup>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}