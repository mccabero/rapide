using AutoMapper;
using Castle.Core.Internal;
using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class MembershipService(IMembershipRepo repo) : BaseService<Membership, MembershipDTO>(repo), IMembershipService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Membership, MembershipDTO>();
                cfg.CreateMap<Customer, CustomerDTO>();

                cfg.CreateMap<MembershipDTO, Membership>();
                cfg.CreateMap<CustomerDTO, Customer>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<MembershipDTO>> GetAllMembershipAsync()
        {
            try
            {
                List<MembershipDTO> dtoList = new List<MembershipDTO>();
                var entityList = await repo.GetAllMembershipAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<MembershipDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<MembershipDTO?> GetAsync(Expression<Func<Membership, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<MembershipDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<MembershipDTO> CreateAsync(MembershipDTO dto)
        {
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            dto.Customer = null;

            var createdDto = await base.CreateAsync(dto);

            return createdDto;
        }

        public override async Task<MembershipDTO> UpdateAsync(MembershipDTO dto)
        {
            try
            {
                // Convert everything to uppercase.
                dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

                IMapper mapper = InitializeMapper();

                // Remove FKs. only parent table is to be updated
                dto.Customer = null;

                var dtoMap = mapper.Map<Membership>(dto);

                await base.UpdateByEntityAsync(dtoMap);

                return dto;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
