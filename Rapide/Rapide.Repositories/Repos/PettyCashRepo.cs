using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class PettyCashRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<PettyCash>(context), IPettyCashRepo
    {
        public async Task<List<PettyCash>> GetAllPettyCashAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PettyCash>()
                .Include(x => x.PaidByUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PettyCash?> GetPettyCashByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PettyCash>()
                .Include(x => x.PaidByUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}