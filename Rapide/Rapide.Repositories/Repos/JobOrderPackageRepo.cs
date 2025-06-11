using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class JobOrderPackageRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<JobOrderPackage>(context), IJobOrderPackageRepo
    {
        public async Task<List<JobOrderPackage>> GetAllJobOrderPackageAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderPackage>()
                .Include(x => x.JobOrder)
                .Include(x => x.Package)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<JobOrderPackage>> GetAllJobOrderPackageByJobOrderIdAsync(int jobOrderId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderPackage>()
                .Include(x => x.JobOrder)
                .Include(x => x.Package)
                .Where(x => x.JobOrder.Id == jobOrderId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<JobOrderPackage?> GetJobOrderPackageByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderPackage>()
                .Include(x => x.JobOrder)
                .Include(x => x.Package)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}