using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Contracts.Services
{
    public interface ICompanyInfoService : IBaseService<CompanyInfo, CompanyInfoDTO>
    {
        Task<List<CompanyInfoDTO>> GetAllAsync();
    }
}
