using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class VehicleRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Vehicle>(context), IVehicleRepo
    {
        public async Task<List<Vehicle>> GetAllVehicleAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Vehicle>()
                .Include(x => x.Customer)
                .Include(x => x.VehicleModel)
                    .ThenInclude(x => x.VehicleMake)
                    .ThenInclude(x => x.RegionParameter)
                .Include(x => x.VehicleModel.BodyParameter)
                .Include(x => x.VehicleModel.ClassificationParameter)
                .Include(x => x.TransmissionParameter)
                .Include(x => x.OdometerParameter)
                .Include(x => x.CustomerRegistrationTypeParameter)
                .Include(x => x.EngineSizeParameter)
                .Include(x => x.EngineTypeParameter)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Vehicle?> GetVehicleByModelAsync(string model)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Vehicle>()
                .Include(x => x.Customer)
                .Include(x => x.VehicleModel)
                    .ThenInclude(x => x.VehicleMake)
                    .ThenInclude(x => x.RegionParameter)
                .Include(x => x.VehicleModel.BodyParameter)
                .Include(x => x.VehicleModel.ClassificationParameter)
                .Include(x => x.TransmissionParameter)
                .Include(x => x.OdometerParameter)
                .Include(x => x.CustomerRegistrationTypeParameter)
                .Include(x => x.EngineSizeParameter)
                .Include(x => x.EngineTypeParameter)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.VehicleModel.Name.Contains(model));
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Vehicle>()
                .Include(x => x.Customer)
                .Include(x => x.VehicleModel)
                    .ThenInclude(x => x.VehicleMake)
                    .ThenInclude(x => x.RegionParameter)
                .Include(x => x.VehicleModel.BodyParameter)
                .Include(x => x.VehicleModel.ClassificationParameter)
                .Include(x => x.TransmissionParameter)
                .Include(x => x.OdometerParameter)
                .Include(x => x.CustomerRegistrationTypeParameter)
                .Include(x => x.EngineSizeParameter)
                .Include(x => x.EngineTypeParameter)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Vehicle>> GetAllVehicleByCustomerIdAsync(int customerId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Vehicle>()
                .Include(x => x.Customer)
                .Include(x => x.VehicleModel)
                    .ThenInclude(x => x.VehicleMake)
                    .ThenInclude(x => x.RegionParameter)
                .Include(x => x.VehicleModel.BodyParameter)
                .Include(x => x.VehicleModel.ClassificationParameter)
                .Include(x => x.TransmissionParameter)
                .Include(x => x.OdometerParameter)
                .Include(x => x.CustomerRegistrationTypeParameter)
                .Include(x => x.EngineSizeParameter)
                .Include(x => x.EngineTypeParameter)
                .Where(x => x.CustomerId == customerId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}