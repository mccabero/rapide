using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IQuickSalesRepo : IBaseRepo<QuickSales>
    {
        Task<QuickSales?> GetQuickSalesByIdAsync(int id);

        Task<List<QuickSales>> GetAllQuickSalesAsync();

        Task<List<QuickSales>> GetAllQuickSalesByCustomerIdAsync(int customerId);
    }
}