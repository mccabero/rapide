using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class EstimatePackageRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<EstimatePackage>(context), IEstimatePackageRepo
    {
        public async Task<List<EstimatePackage>> GetAllEstimatePackageAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimatePackage>()
                .Include(x => x.Estimate)
                .Include(x => x.Package)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<EstimatePackage>> GetAllEstimatePackageByEstimateIdAsync(int estimateId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimatePackage>()
                .Include(x => x.Estimate)
                .Include(x => x.Package)
                .Where(x => x.Estimate.Id == estimateId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<EstimatePackage?> GetEstimatePackageByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimatePackage>()
                .Include(x => x.Estimate)
                .Include(x => x.Package)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}