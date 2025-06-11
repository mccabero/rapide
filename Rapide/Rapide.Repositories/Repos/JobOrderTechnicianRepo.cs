using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class JobOrderTechnicianRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<JobOrderTechnician>(context), IJobOrderTechnicianRepo
    {
        public async Task<List<JobOrderTechnician>> GetAllJobOrderTechnicianAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderTechnician>()
                .Include(x => x.TechnicianUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<JobOrderTechnician>> GetAllJobOrderTechnicianByJobOrderIdAsync(int estimateId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderTechnician>()
                .Include(x => x.TechnicianUser)
                    .ThenInclude(x => x.Role)
                .Where(x => x.JobOrderId == estimateId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<JobOrderTechnician?> GetJobOrderTechnicianByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrderTechnician>()
                .Include(x => x.TechnicianUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}