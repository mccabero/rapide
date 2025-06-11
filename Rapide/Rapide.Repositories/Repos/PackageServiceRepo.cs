using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class PackageServiceRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<PackageService>(context), IPackageServiceRepo
    {
        public async Task<List<PackageService>> GetAllPackageServiceAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PackageService>()
                .Include(x => x.Package)
                .Include(x => x.Service)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PackageService>> GetAllPackageServiceByPackageIdAsync(int packageId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PackageService>()
                .Include(x => x.Package)
                .Include(x => x.Service)
                .Where(x => x.PackageId == packageId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PackageService?> GetPackageServiceByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PackageService>()
                .Include(x => x.Package)
                .Include(x => x.Service)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}