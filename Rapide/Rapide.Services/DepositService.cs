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
    public class DepositService(IDepositRepo repo) : BaseService<Deposit, DepositDTO>(repo), IDepositService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Deposit, DepositDTO>();
                cfg.CreateMap<JobStatus, JobStatusDTO>();
                cfg.CreateMap<Customer, CustomerDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<ParameterGroup, ParameterGroupDTO>();
                cfg.CreateMap<JobOrder, JobOrderDTO>();
                //cfg.CreateMap<Estimate, EstimateDTO>();
                //cfg.CreateMap<Vehicle, VehicleDTO>();

                //cfg.CreateMap<VehicleDTO, Vehicle>();
                //cfg.CreateMap<EstimateDTO, Estimate>();
                cfg.CreateMap<DepositDTO, Deposit>();
                cfg.CreateMap<JobStatusDTO, JobStatus>();
                cfg.CreateMap<CustomerDTO, Customer>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroup>();
                cfg.CreateMap<JobOrderDTO, JobOrder>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<DepositDTO>> GetAllDepositAsync()
        {
            try
            {
                List<DepositDTO> dtoList = new List<DepositDTO>();
                var entityList = await repo.GetAllDepositAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<DepositDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<DepositDTO?> GetAsync(Expression<Func<Deposit, bool>> predicate)
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

        public async Task<DepositDTO?> GetDepositByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetDepositByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<DepositDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<DepositDTO> CreateAsync(DepositDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.JobStatus = null;
            dto.Customer = null;
            dto.JobOrder = null;
            dto.PaymentTypeParameter = null;

            var createdDto = await base.CreateAsync(dto);

            return createdDto;
        }

        public override async Task<DepositDTO> UpdateAsync(DepositDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            dto.JobStatus = null;
            dto.Customer = null;
            dto.JobOrder = null;
            dto.PaymentTypeParameter = null;

            var dtoMap = mapper.Map<Deposit>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }

        public async Task<List<DepositDTO>> GetAllDepositByCustomerIdAsync(int customerId)
        {
            try
            {
                List<DepositDTO> dtoList = new List<DepositDTO>();
                var entityList = await repo.GetAllDepositByCustomerIdAsync(customerId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<DepositDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<DepositDTO>> GetAllDepositByJobOrderIdAsync(int jobOrderId)
        {
            try
            {
                List<DepositDTO> dtoList = new List<DepositDTO>();
                var entityList = await repo.GetAllDepositByJobOrderIdAsync(jobOrderId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<DepositDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}