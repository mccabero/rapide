using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class DepositRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Deposit>(context), IDepositRepo
    {
        public async Task<List<Deposit>> GetAllDepositAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Deposit>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                .Include(x => x.JobOrder)
                .Include(x => x.PaymentTypeParameter)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Deposit>> GetAllDepositByCustomerIdAsync(int customerId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Deposit>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                .Include(x => x.JobOrder)
                .Include(x => x.PaymentTypeParameter)
                .Where(x => x.Customer.Id == customerId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Deposit>> GetAllDepositByJobOrderIdAsync(int jobOrderId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Deposit>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                .Include(x => x.JobOrder)
                .Include(x => x.PaymentTypeParameter)
                .Where(x => x.JobOrder.Id == jobOrderId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Deposit?> GetDepositByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Deposit>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                .Include(x => x.JobOrder)
                .Include(x => x.PaymentTypeParameter)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}