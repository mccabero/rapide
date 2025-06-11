using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface IQuickSalesProductRepo : IBaseRepo<QuickSalesProduct>
    {
        Task<QuickSalesProduct?> GetQuickSalesProductByIdAsync(int id);

        Task<List<QuickSalesProduct>> GetAllQuickSalesProductAsync();

        Task<List<QuickSalesProduct>> GetAllQuickSalesProductByQuickSalesIdAsync(int quickSalesId);
    }
}