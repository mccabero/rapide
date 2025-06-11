using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class JobStatusRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<JobStatus>(context), IJobStatusRepo
    {
        public async Task<List<JobStatus>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<JobStatus>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}