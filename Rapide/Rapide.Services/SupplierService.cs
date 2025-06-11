using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class SupplierService(ISupplierRepo repo) : BaseService<Supplier, SupplierDTO>(repo), ISupplierService
    {
        public async Task<List<SupplierDTO>> GetAllAsync()
        {
            try
            {
                List<SupplierDTO> dtoList = new List<SupplierDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                foreach (var e in entity)
                    dtoList.Add(e.Map<SupplierDTO>());
                
                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<SupplierDTO?> GetAsync(Expression<Func<Supplier, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<SupplierDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}