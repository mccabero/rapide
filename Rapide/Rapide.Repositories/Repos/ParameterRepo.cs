using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class ParameterRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Parameter>(context), IParameterRepo
    {
        public async Task<List<Parameter>> GetAllParameterAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Parameter>()
                .Include(x => x.ParameterGroup)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Parameter?> GetParameterByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Parameter>()
                .Include(x => x.ParameterGroup)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}