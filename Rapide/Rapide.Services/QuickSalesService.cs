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
    public class QuickSalesService(IQuickSalesRepo repo) : BaseService<QuickSales, QuickSalesDTO>(repo), IQuickSalesService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<QuickSales, QuickSalesDTO>();
                cfg.CreateMap<JobStatus, JobStatusDTO>();
                cfg.CreateMap<Customer, CustomerDTO>();
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<ParameterGroup, ParameterGroupDTO>();
                cfg.CreateMap<Role, RoleDTO>();

                cfg.CreateMap<RoleDTO, Role>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroup>();
                cfg.CreateMap<QuickSalesDTO, QuickSales>();
                cfg.CreateMap<JobStatusDTO, JobStatus>();
                cfg.CreateMap<CustomerDTO, Customer>();
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<ServiceGroupDTO, ServiceGroup>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<QuickSalesDTO>> GetAllQuickSalesAsync()
        {
            try
            {
                List<QuickSalesDTO> dtoList = new List<QuickSalesDTO>();
                var entityList = await repo.GetAllQuickSalesAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<QuickSalesDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<QuickSalesDTO?> GetAsync(Expression<Func<QuickSales, bool>> predicate)
        {
            try
            {
                var dto = await base.GetAsync(predicate);

                if (dto == null)
                    return null;

                return dto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<QuickSalesDTO?> GetQuickSalesByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetQuickSalesByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<QuickSalesDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<QuickSalesDTO> CreateAsync(QuickSalesDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.JobStatus = null;
            dto.Customer = null;
            dto.PaymentTypeParameter = null;
            dto.SalesPersonUser = null;

            var createdDto = await base.CreateAsync(dto);

            return createdDto;
        }

        public override async Task<QuickSalesDTO> UpdateAsync(QuickSalesDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.JobStatus = null;
            dto.Customer = null;
            dto.PaymentTypeParameter = null;
            dto.SalesPersonUser = null;

            var dtoMap = mapper.Map<QuickSales>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }

        public async Task<List<QuickSalesDTO>> GetAllQuickSalesByCustomerIdAsync(int customerId)
        {
            try
            {
                List<QuickSalesDTO> dtoList = new List<QuickSalesDTO>();
                var entityList = await repo.GetAllQuickSalesByCustomerIdAsync(customerId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<QuickSalesDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}