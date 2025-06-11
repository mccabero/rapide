using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface IQuickSalesProductService : IBaseService<QuickSalesProduct, QuickSalesProductDTO>
    {
        Task<QuickSalesProductDTO?> GetQuickSalesProductByIdAsync(int id);

        Task<List<QuickSalesProductDTO>> GetAllQuickSalesProductAsync();

        Task<List<QuickSalesProductDTO>> GetAllQuickSalesProductByQuickSalesIdAsync(int quickSalesId);
    }
}
