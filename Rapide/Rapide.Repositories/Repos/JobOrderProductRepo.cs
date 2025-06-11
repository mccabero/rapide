using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class JobOrderProductRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<JobOrderProduct>(context), IJobOrderProductRepo
    {
        public async Task<List<JobOrderProduct>> GetAllJobOrderProductAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderProduct>()
                .Include(x => x.Product)
                    .ThenInclude(x => x.UnitOfMeasure)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<JobOrderProduct>> GetAllJobOrderProductByJobOrderIdAsync(int jobOrderId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderProduct>()
                .Include(x => x.Product)
                    .ThenInclude(x => x.UnitOfMeasure)
                .Where(x => x.JobOrderId == jobOrderId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<JobOrderProduct?> GetJobOrderProductByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderProduct>()
                .Include(x => x.Product)
                    .ThenInclude(x => x.UnitOfMeasure)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}