using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class SupplierRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Supplier>(context), ISupplierRepo
    {
        public async Task<List<Supplier>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<Supplier>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}