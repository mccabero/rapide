using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class ManufacturerRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Manufacturer>(context), IManufacturerRepo
    {
        public async Task<List<Manufacturer>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<Manufacturer>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}