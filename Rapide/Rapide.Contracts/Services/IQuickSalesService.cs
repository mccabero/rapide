using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IQuickSalesService : IBaseService<QuickSales, QuickSalesDTO>
    {
        Task<QuickSalesDTO?> GetQuickSalesByIdAsync(int id);

        Task<List<QuickSalesDTO>> GetAllQuickSalesAsync();

        Task<List<QuickSalesDTO>> GetAllQuickSalesByCustomerIdAsync(int customerId);
    }
}
