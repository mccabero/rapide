using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class PackageProductRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<PackageProduct>(context), IPackageProductRepo
    {
        public async Task<List<PackageProduct>> GetAllPackageProductAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PackageProduct>()
                .Include(x => x.Package)
                .Include(x => x.Product)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PackageProduct>> GetAllPackageProductByPackageIdAsync(int packageId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PackageProduct>()
                .Include(x => x.Package)
                .Include(x => x.Product)
                .Where(x => x.PackageId == packageId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PackageProduct?> GetPackageProductByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PackageProduct>()
                .Include(x => x.Package)
                .Include(x => x.Product)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}