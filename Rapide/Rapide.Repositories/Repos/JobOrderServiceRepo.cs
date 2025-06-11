using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class JobOrderServiceRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<JobOrderService>(context), IJobOrderServiceRepo
    {
        public async Task<List<JobOrderService>> GetAllJobOrderServiceAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderService>()
                .Include(x => x.Service)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<JobOrderService>> GetAllJobOrderServiceByJobOrderIdAsync(int jobOrderId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderService>()
                .Include(x => x.JobOrder)
                .Include(x => x.Service)
                .Where(x => x.JobOrderId == jobOrderId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<JobOrderService?> GetJobOrderServiceByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderService>()
                .Include(x => x.Service)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}