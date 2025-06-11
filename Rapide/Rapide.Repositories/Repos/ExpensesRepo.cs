using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class ExpensesRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Expenses>(context), IExpensesRepo
    {
        public async Task<List<Expenses>> GetAllExpensesAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Expenses>()
                .Include(x => x.JobStatus)
                .Include(x => x.PaymentTypeParameter)
                .Include(x => x.ExpenseByUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Expenses>> GetAllExpensesByExpenseByUserIdAsync(int expenseByUserId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Expenses>()
                .Include(x => x.JobStatus)
                .Include(x => x.PaymentTypeParameter)
                .Include(x => x.ExpenseByUser)
                    .ThenInclude(x => x.Role)
                .Where(x => x.ExpenseByUser.Id == expenseByUserId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Expenses?> GetExpensesByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Expenses>()
                .Include(x => x.JobStatus)
                .Include(x => x.PaymentTypeParameter)
                .Include(x => x.ExpenseByUser)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}