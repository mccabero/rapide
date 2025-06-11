using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class EstimateServiceRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<EstimateService>(context), IEstimateServiceRepo
    {
        public async Task<List<EstimateService>> GetAllEstimateServiceAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimateService>()
                .Include(x => x.Estimate)
                .Include(x => x.Service)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<EstimateService>> GetAllEstimateServiceByEstimateIdAsync(int estimateId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimateService>()
                .Include(x => x.Estimate)
                .Include(x => x.Service)
                .Where(x => x.EstimateId == estimateId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<EstimateService?> GetEstimateServiceByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimateService>()
                .Include(x => x.Estimate)
                .Include(x => x.Service)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}