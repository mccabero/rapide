using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IExpensesService : IBaseService<Expenses, ExpensesDTO>
    {
        Task<ExpensesDTO?> GetExpensesByIdAsync(int id);

        Task<List<ExpensesDTO>> GetAllExpensesAsync();

        Task<List<ExpensesDTO>> GetAllExpensesByExpenseByUserIdAsync(int expenseByUserId);
    }
}
