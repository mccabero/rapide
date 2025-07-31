using AutoMapper;
using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class CustomerService(ICustomerRepo repo) : BaseService<Customer, CustomerDTO>(repo), ICustomerService
    {
        public async Task<List<CustomerDTO>> GetAllAsync()
        {
            try
            {
                List<CustomerDTO> dtoList = new List<CustomerDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                IMapper mapper = MappingHelper.InitializeMapper();
                dtoList = mapper.Map<List<CustomerDTO>>(entity);

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<CustomerDTO?> GetAsync(Expression<Func<Customer, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<CustomerDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}