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
                .Include(x => x.JobStatus)
                .Include(x => x.ApprovedByUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.PaidByUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PettyCash?> GetPettyCashByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PettyCash>()
                .Include(x => x.JobStatus)
                .Include(x => x.ApprovedByUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.PaidByUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<PettyCashDetails>> GetAllPettyCashDetailsAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PettyCashDetails>()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PettyCashDetails?> GetPettyCashDetailsByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PettyCashDetails>()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}