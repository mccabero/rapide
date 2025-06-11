using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class PackageRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Package>(context), IPackageRepo
    {
        public async Task<List<Package>> GetAllPackageAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<Package>()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Package?> GetPackageByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Package>()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}