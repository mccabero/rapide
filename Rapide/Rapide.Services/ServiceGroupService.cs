using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class ServiceGroupService(IServiceGroupRepo repo) : BaseService<ServiceGroup, ServiceGroupDTO>(repo), IServiceGroupService
    {
        public async Task<List<ServiceGroupDTO>> GetAllAsync()
        {
            try
            {
                List<ServiceGroupDTO> dtoList = new List<ServiceGroupDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                foreach (var e in entity)
                    dtoList.Add(e.Map<ServiceGroupDTO>());
                
                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ServiceGroupDTO?> GetAsync(Expression<Func<ServiceGroup, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<ServiceGroupDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}