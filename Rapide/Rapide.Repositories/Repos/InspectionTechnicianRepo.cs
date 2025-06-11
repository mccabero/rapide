using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class InspectionTechnicianRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<InspectionTechnician>(context), IInspectionTechnicianRepo
    {
        public async Task<List<InspectionTechnician>> GetAllInspectionTechnicianAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<InspectionTechnician>()
                .Include(x => x.TechnicianUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<InspectionTechnician>> GetAllInspectionTechnicianByInspectionIdAsync(int inspectionId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<InspectionTechnician>()
                .Include(x => x.TechnicianUser)
                    .ThenInclude(x => x.Role)
                .Where(x => x.InspectionId == inspectionId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<InspectionTechnician?> GetInspectionTechnicianByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<InspectionTechnician>()
                .Include(x => x.TechnicianUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}