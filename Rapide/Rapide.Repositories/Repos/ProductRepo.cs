using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class ProductRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Product>(context), IProductRepo
    {
        public async Task<List<Product>> GetAllProductAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Product>()
                .Include(x => x.ProductGroup)
                .Include(x => x.ProductCategory)
                .Include(x => x.UnitOfMeasure)
                .Include(x => x.Manufacturer)
                .Include(x => x.Supplier)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetProductByPartNoAsync(string partNo)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Product>()
                .Include(x => x.ProductGroup)
                .Include(x => x.ProductCategory)
                .Include(x => x.UnitOfMeasure)
                .Include(x => x.Manufacturer)
                .Include(x => x.Supplier)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.PartNo == partNo);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Product>()
                .Include(x => x.ProductGroup)
                .Include(x => x.ProductCategory)
                .Include(x => x.UnitOfMeasure)
                .Include(x => x.Manufacturer)
                .Include(x => x.Supplier)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}