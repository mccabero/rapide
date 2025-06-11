using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class PaymentDetailsRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<PaymentDetails>(context), IPaymentDetailsRepo
    {
        public async Task<List<PaymentDetails>> GetAllPaymentDetailsAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PaymentDetails>()
                .Include(x => x.Payment)
                .Include(x => x.Invoice)
                .Include(x => x.PaymentTypeParameter)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PaymentDetails>> GetAllPaymentDetailsByPaymentIdAsync(int paymentId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PaymentDetails>()
                .Include(x => x.Payment)
                .Include(x => x.Invoice)
                .Include(x => x.PaymentTypeParameter)
                .Where(x => x.Payment.Id == paymentId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PaymentDetails>> GetAllPaymentDetailsByInvoiceIdAsync(int invoiceId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PaymentDetails>()
                .Include(x => x.Payment)
                .Include(x => x.Invoice)
                .Include(x => x.PaymentTypeParameter)
                .Where(x => x.Invoice.Id == invoiceId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PaymentDetails?> GetPaymentDetailsByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<PaymentDetails>()
                .Include(x => x.Payment)
                .Include(x => x.Invoice)
                .Include(x => x.PaymentTypeParameter)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}