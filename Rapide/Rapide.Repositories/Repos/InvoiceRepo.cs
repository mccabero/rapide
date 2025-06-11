using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class InvoiceRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Invoice>(context), IInvoiceRepo
    {
        public async Task<List<Invoice>> GetAllInvoiceAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Invoice>()
                .Include(x => x.JobStatus)
                .Include(x => x.JobOrder)
                .Include(x => x.JobOrder.Customer)
                .Include(x => x.JobOrder.AdvisorUser)
                .Include(x => x.JobOrder.Estimate)
                .Include(x => x.JobOrder.EstimatorUser)
                .Include(x => x.JobOrder.JobStatus)
                .Include(x => x.JobOrder.ServiceGroup)
                .Include(x => x.JobOrder.Vehicle)
                .Include(x => x.JobOrder.Vehicle.EngineSizeParameter)
                .Include(x => x.JobOrder.Vehicle.TransmissionParameter)
                .Include(x => x.JobOrder.Vehicle.VehicleModel)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.ClassificationParameter)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.BodyParameter)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.VehicleMake)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.VehicleMake.RegionParameter)
                .Include(x => x.Customer)
                .Include(x => x.JobOrder.Customer)
                .Include(x => x.JobOrder.EstimatorUser)
                .Include(x => x.JobOrder.EstimatorUser.Role)
                .Include(x => x.JobOrder.ApproverUser)
                .Include(x => x.JobOrder.ApproverUser.Role)
                .Include(x => x.AdvisorUser)
                .Include(x => x.AdvisorUser.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetAllInvoiceByCustomerIdAsync(int customerId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Invoice>()
                .Include(x => x.JobStatus)
                .Include(x => x.JobOrder)
                .Include(x => x.JobOrder.Customer)
                .Include(x => x.JobOrder.AdvisorUser)
                .Include(x => x.JobOrder.Estimate)
                .Include(x => x.JobOrder.EstimatorUser)
                .Include(x => x.JobOrder.JobStatus)
                .Include(x => x.JobOrder.ServiceGroup)
                .Include(x => x.JobOrder.Vehicle)
                .Include(x => x.JobOrder.Vehicle.EngineSizeParameter)
                .Include(x => x.JobOrder.Vehicle.TransmissionParameter)
                .Include(x => x.JobOrder.Vehicle.VehicleModel)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.ClassificationParameter)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.BodyParameter)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.VehicleMake)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.VehicleMake.RegionParameter)
                .Include(x => x.Customer)
                .Include(x => x.JobOrder.Customer)
                .Include(x => x.JobOrder.EstimatorUser)
                .Include(x => x.JobOrder.EstimatorUser.Role)
                .Include(x => x.JobOrder.ApproverUser)
                .Include(x => x.JobOrder.ApproverUser.Role)
                .Include(x => x.AdvisorUser)
                .Include(x => x.AdvisorUser.Role)
                .Where(x => x.Customer.Id == customerId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Invoice>()
                .Include(x => x.JobStatus)
                .Include(x => x.JobOrder)
                .Include(x => x.JobOrder.Customer)
                .Include(x => x.JobOrder.AdvisorUser)
                .Include(x => x.JobOrder.Estimate)
                .Include(x => x.JobOrder.EstimatorUser)
                .Include(x => x.JobOrder.JobStatus)
                .Include(x => x.JobOrder.ServiceGroup)
                .Include(x => x.JobOrder.Vehicle)
                .Include(x => x.JobOrder.Vehicle.EngineSizeParameter)
                .Include(x => x.JobOrder.Vehicle.TransmissionParameter)
                .Include(x => x.JobOrder.Vehicle.VehicleModel)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.ClassificationParameter)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.BodyParameter)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.VehicleMake)
                .Include(x => x.JobOrder.Vehicle.VehicleModel.VehicleMake.RegionParameter)
                .Include(x => x.Customer)
                .Include(x => x.JobOrder.Customer)
                .Include(x => x.JobOrder.EstimatorUser)
                .Include(x => x.JobOrder.EstimatorUser.Role)
                .Include(x => x.JobOrder.ApproverUser)
                .Include(x => x.JobOrder.ApproverUser.Role)
                .Include(x => x.AdvisorUser)
                .Include(x => x.AdvisorUser.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}