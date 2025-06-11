using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class VehicleMakeRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<VehicleMake>(context), IVehicleMakeRepo
    {
        public async Task<List<VehicleMake>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<VehicleMake>()
                .Include(x => x.RegionParameter)
                    .ThenInclude(x => x.ParameterGroup)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<VehicleMake?> GetVehicleMakeByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<VehicleMake>()
                .Include(x => x.RegionParameter)
                    .ThenInclude(x => x.ParameterGroup)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}