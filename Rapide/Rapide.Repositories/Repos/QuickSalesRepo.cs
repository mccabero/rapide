using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class QuickSalesRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<QuickSales>(context), IQuickSalesRepo
    {
        public async Task<List<QuickSales>> GetAllQuickSalesAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<QuickSales>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                .Include(x => x.PaymentTypeParameter)
                .Include(x => x.SalesPersonUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<QuickSales>> GetAllQuickSalesByCustomerIdAsync(int customerId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<QuickSales>()
                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                .Include(x => x.PaymentTypeParameter)
                .Include(x => x.SalesPersonUser)
                    .ThenInclude(x => x.Role)
                .Where(x => x.Customer.Id == customerId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<QuickSales?> GetQuickSalesByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<QuickSales>()
                                .Include(x => x.JobStatus)
                .Include(x => x.Customer)
                .Include(x => x.PaymentTypeParameter)
                .Include(x => x.SalesPersonUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}