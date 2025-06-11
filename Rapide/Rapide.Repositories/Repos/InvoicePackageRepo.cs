using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class InvoicePackageRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<InvoicePackage>(context), IInvoicePackageRepo
    {
        public async Task<List<InvoicePackage>> GetAllInvoicePackageAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<InvoicePackage>()
                .Include(x => x.Invoice)
                .Include(x => x.Package)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<InvoicePackage>> GetAllInvoicePackageByInvoiceIdAsync(int invoiceId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<InvoicePackage>()
                .Include(x => x.Invoice)
                .Include(x => x.Package)
                .Where(x => x.Invoice.Id == invoiceId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<InvoicePackage?> GetInvoicePackageByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<InvoicePackage>()
                .Include(x => x.Invoice)
                .Include(x => x.Package)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}