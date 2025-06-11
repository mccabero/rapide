using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class VehicleModelRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<VehicleModel>(context), IVehicleModelRepo
    {
        public async Task<List<VehicleModel>> GetAllVehicleModelAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<VehicleModel>()
                .Include(x => x.VehicleMake)
                .Include(x => x.BodyParameter)
                    .ThenInclude(x => x.ParameterGroup)
                .Include(x => x.ClassificationParameter)
                    .ThenInclude(x => x.ParameterGroup)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<VehicleModel?> GetVehicleModelByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<VehicleModel>()
                .Include(x => x.VehicleMake)
                .Include(x => x.BodyParameter)
                    .ThenInclude(x => x.ParameterGroup)
                .Include(x => x.ClassificationParameter)
                    .ThenInclude(x => x.ParameterGroup)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}