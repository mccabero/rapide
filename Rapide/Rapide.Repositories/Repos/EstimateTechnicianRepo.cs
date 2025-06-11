using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class EstimateTechnicianRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<EstimateTechnician>(context), IEstimateTechnicianRepo
    {
        public async Task<List<EstimateTechnician>> GetAllEstimateTechnicianAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimateTechnician>()
                .Include(x => x.TechnicianUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<EstimateTechnician>> GetAllEstimateTechnicianByEstimateIdAsync(int estimateId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimateTechnician>()
                .Include(x => x.TechnicianUser)
                    .ThenInclude(x => x.Role)
                .Where(x => x.EstimateId == estimateId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<EstimateTechnician?> GetEstimateTechnicianByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<EstimateTechnician>()
                .Include(x => x.TechnicianUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}