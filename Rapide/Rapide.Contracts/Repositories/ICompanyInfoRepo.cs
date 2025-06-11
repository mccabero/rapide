using Rapide.Entities;

namespace Rapide.Contracts.Repositories
{
    public interface ICompanyInfoRepo : IBaseRepo<CompanyInfo>
    {
        Task<List<CompanyInfo>> GetAllAsync();
    }
}