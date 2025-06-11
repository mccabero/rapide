using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class CompanyInfoService(ICompanyInfoRepo repo) : BaseService<CompanyInfo, CompanyInfoDTO>(repo), ICompanyInfoService
    {
        public async Task<List<CompanyInfoDTO>> GetAllAsync()
        {
            try
            {
                List<CompanyInfoDTO> dtoList = new List<CompanyInfoDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                foreach (var e in entity)
                    dtoList.Add(e.Map<CompanyInfoDTO>());

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<CompanyInfoDTO?> GetAsync(Expression<Func<CompanyInfo, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<CompanyInfoDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}