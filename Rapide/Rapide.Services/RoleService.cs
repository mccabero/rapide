using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class RoleService(IRoleRepo repo) : BaseService<Role, RoleDTO>(repo), IRoleService
    {
        public async Task<List<RoleDTO>> GetAllAsync()
        {
            try
            {
                List<RoleDTO> dtoList = new List<RoleDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                foreach (var e in entity)
                    dtoList.Add(e.Map<RoleDTO>());
                
                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<RoleDTO?> GetAsync(Expression<Func<Role, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<RoleDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}