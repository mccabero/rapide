using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class ParameterGroupService(IParameterGroupRepo repo) : BaseService<ParameterGroup, ParameterGroupDTO>(repo), IParameterGroupService
    {
        public async Task<List<ParameterGroupDTO>> GetAllAsync()
        {
            try
            {
                List<ParameterGroupDTO> dtoList = new List<ParameterGroupDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                foreach (var e in entity)
                    dtoList.Add(e.Map<ParameterGroupDTO>());
                
                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public override async Task<ParameterGroupDTO?> GetAsync(Expression<Func<ParameterGroup, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<ParameterGroupDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }           
        }
    }
}