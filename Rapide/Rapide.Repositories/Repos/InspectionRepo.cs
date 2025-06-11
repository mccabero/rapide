using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class InspectionRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Inspection>(context), IInspectionRepo
    {
        public async Task<List<Inspection>> GetAllInspectionAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Inspection>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                 .Include(x => x.AdvisorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.EstimatorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ApproverUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.InspectorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ServiceGroup)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                    .ThenInclude(x => x.VehicleMake)
                .Include(x => x.Vehicle.TransmissionParameter)
                .Include(x => x.Vehicle.CustomerRegistrationTypeParameter)
                .Include(x => x.Vehicle.EngineSizeParameter)
                .Include(x => x.Vehicle.EngineTypeParameter)
                .Include(x => x.Vehicle.OdometerParameter)
                .Include(x => x.Vehicle.VehicleModel.BodyParameter)
                .Include(x => x.Vehicle.VehicleModel.ClassificationParameter)
                .Include(x => x.InspectorUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Inspection>> GetAllInspectionByCustomerIdAsync(int customerId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Inspection>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                 .Include(x => x.AdvisorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.EstimatorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ApproverUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.InspectorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ServiceGroup)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                    .ThenInclude(x => x.VehicleMake)
                .Include(x => x.Vehicle.TransmissionParameter)
                .Include(x => x.Vehicle.CustomerRegistrationTypeParameter)
                .Include(x => x.Vehicle.EngineSizeParameter)
                .Include(x => x.Vehicle.EngineTypeParameter)
                .Include(x => x.Vehicle.OdometerParameter)
                .Include(x => x.Vehicle.VehicleModel.BodyParameter)
                .Include(x => x.Vehicle.VehicleModel.ClassificationParameter)
                .Include(x => x.InspectorUser)
                    .ThenInclude(x => x.Role)
                .Where(x => x.Customer.Id == customerId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Inspection>> GetAllInspectionByVehicleIdAsync(int vehicleId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Inspection>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                 .Include(x => x.AdvisorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.EstimatorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ApproverUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.InspectorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ServiceGroup)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                    .ThenInclude(x => x.VehicleMake)
                .Include(x => x.Vehicle.TransmissionParameter)
                .Include(x => x.Vehicle.CustomerRegistrationTypeParameter)
                .Include(x => x.Vehicle.EngineSizeParameter)
                .Include(x => x.Vehicle.EngineTypeParameter)
                .Include(x => x.Vehicle.OdometerParameter)
                .Include(x => x.Vehicle.VehicleModel.BodyParameter)
                .Include(x => x.Vehicle.VehicleModel.ClassificationParameter)
                .Include(x => x.InspectorUser)
                    .ThenInclude(x => x.Role)
                .Where(x => x.Vehicle.Id == vehicleId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Inspection?> GetInspectionByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Inspection>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                 .Include(x => x.AdvisorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.EstimatorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ApproverUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.InspectorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ServiceGroup)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                    .ThenInclude(x => x.VehicleMake)
                .Include(x => x.Vehicle.TransmissionParameter)
                .Include(x => x.Vehicle.CustomerRegistrationTypeParameter)
                .Include(x => x.Vehicle.EngineSizeParameter)
                .Include(x => x.Vehicle.EngineTypeParameter)
                .Include(x => x.Vehicle.OdometerParameter)
                .Include(x => x.InspectorUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}