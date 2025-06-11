using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class QuickSalesProductRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<QuickSalesProduct>(context), IQuickSalesProductRepo
    {
        public async Task<List<QuickSalesProduct>> GetAllQuickSalesProductAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<QuickSalesProduct>()
                .Include(x => x.Product)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<QuickSalesProduct>> GetAllQuickSalesProductByQuickSalesIdAsync(int quickSalesId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<QuickSalesProduct>()
                .Include(x => x.Product)
                .Where(x => x.QuickSalesId == quickSalesId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<QuickSalesProduct?> GetQuickSalesProductByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<QuickSalesProduct>()
                .Include(x => x.Product)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}