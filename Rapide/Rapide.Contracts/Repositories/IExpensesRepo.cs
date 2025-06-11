using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IExpensesRepo : IBaseRepo<Expenses>
    {
        Task<Expenses?> GetExpensesByIdAsync(int id);

        Task<List<Expenses>> GetAllExpensesAsync();

        Task<List<Expenses>> GetAllExpensesByExpenseByUserIdAsync(int expenseByUserId);
    }
}