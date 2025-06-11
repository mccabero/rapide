using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class JobOrderRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<JobOrder>(context), IJobOrderRepo
    {
        public async Task<List<JobOrder>> GetAllJobOrderAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrder>()
                .Include(x => x.JobStatus)
                .Include(x => x.Estimate)
                .Include(x => x.Customer)
                 .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.BodyParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.ClassificationParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.VehicleMake)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.EngineSizeParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.TransmissionParameter)
                .Include(x => x.AdvisorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.EstimatorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ApproverUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ServiceGroup)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<JobOrder>> GetAllJobOrderByCustomerIdAsync(int customerId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrder>()
                .Include(x => x.JobStatus)
                .Include(x => x.Estimate)
                .Include(x => x.Customer)
                 .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.BodyParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.ClassificationParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.VehicleMake)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.EngineSizeParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.TransmissionParameter)
                .Include(x => x.AdvisorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.EstimatorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ApproverUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ServiceGroup)
                .Where(x => x.Customer.Id == customerId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<JobOrder>> GetAllJobOrderByVehicleIdAsync(int vehicleId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrder>()
                .Include(x => x.JobStatus)
                .Include(x => x.Estimate)
                .Include(x => x.Customer)
                 .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.BodyParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.ClassificationParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.VehicleMake)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.EngineSizeParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.TransmissionParameter)
                .Include(x => x.AdvisorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.EstimatorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ApproverUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ServiceGroup)
                .Where(x => x.Vehicle.Id == vehicleId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<JobOrder?> GetJobOrderByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<JobOrder>()
                .Include(x => x.JobStatus)
                .Include(x => x.Estimate)
                .Include(x => x.Customer)
                 .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.BodyParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.ClassificationParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                        .ThenInclude(x => x.VehicleMake)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.EngineSizeParameter)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.TransmissionParameter)
                .Include(x => x.EstimatorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.AdvisorUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ApproverUser)
                    .ThenInclude(x => x.Role)
                .Include(x => x.ServiceGroup)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}