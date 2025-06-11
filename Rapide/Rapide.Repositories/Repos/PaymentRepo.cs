using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class PaymentRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Payment>(context), IPaymentRepo
    {
        public async Task<List<Payment>> GetAllPaymentAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Payment>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Payment>> GetAllPaymentByCustomerIdAsync(int customerId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Payment>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                .Where(x => x.Customer.Id == customerId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Payment>()
                                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}