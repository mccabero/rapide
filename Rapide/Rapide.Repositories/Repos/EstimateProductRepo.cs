using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class EstimateProductRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<EstimateProduct>(context), IEstimateProductRepo
    {
        public async Task<List<EstimateProduct>> GetAllEstimateProductAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimateProduct>()
                .Include(x => x.Product)
                    .ThenInclude(x => x.UnitOfMeasure)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<EstimateProduct>> GetAllEstimateProductByEstimateIdAsync(int estimateId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimateProduct>()
                .Include(x => x.Product)
                    .ThenInclude(x => x.UnitOfMeasure)
                .Where(x => x.EstimateId == estimateId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<EstimateProduct?> GetEstimateProductByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimateProduct>()
                .Include(x => x.Product)
                    .ThenInclude(x => x.UnitOfMeasure)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}