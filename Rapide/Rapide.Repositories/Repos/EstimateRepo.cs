using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class EstimateRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Estimate>(context), IEstimateRepo
    {
        public async Task<List<Estimate>> GetAllEstimateAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Estimate>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                .Include(x => x.Vehicle)
                    .ThenInclude(x => x.VehicleModel)
                    .ThenInclude(x => x.VehicleMake)
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

        public async Task<List<Estimate>> GetAllEstimateByCustomerIdAsync(int customerId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Estimate>()
                .Include(x => x.JobStatus)
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
                .Where(x => x.Customer.Id == customerId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Estimate?> GetEstimateByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Estimate>()
                .Include(x => x.JobStatus)
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