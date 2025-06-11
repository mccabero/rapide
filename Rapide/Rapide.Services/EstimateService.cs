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
    public class EstimateService(IEstimateRepo repo) : BaseService<Estimate, EstimateDTO>(repo), IEstimateService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Estimate, EstimateDTO>();
                cfg.CreateMap<JobStatus, JobStatusDTO>();
                cfg.CreateMap<Customer, CustomerDTO>();
                cfg.CreateMap<Vehicle, VehicleDTO>();
                cfg.CreateMap<VehicleModel, VehicleModelDTO>();
                cfg.CreateMap<VehicleMake, VehicleMakeDTO>();
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<ServiceGroup, ServiceGroupDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<Role, RoleDTO>();

                cfg.CreateMap<RoleDTO, Role>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<EstimateDTO, Estimate>();
                cfg.CreateMap<JobStatusDTO, JobStatus>();
                cfg.CreateMap<CustomerDTO, Customer>();
                cfg.CreateMap<VehicleDTO, Vehicle>();
                cfg.CreateMap<VehicleModelDTO, VehicleModel>();
                cfg.CreateMap<VehicleMakeDTO, VehicleMake>();
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<ServiceGroupDTO, ServiceGroup>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<EstimateDTO>> GetAllEstimateAsync()
        {
            try
            {
                List<EstimateDTO> dtoList = new List<EstimateDTO>();
                var entityList = await repo.GetAllEstimateAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<EstimateDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<EstimateDTO?> GetAsync(Expression<Func<Estimate, bool>> predicate)
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

        public async Task<EstimateDTO?> GetEstimateByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetEstimateByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<EstimateDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<EstimateDTO> CreateAsync(EstimateDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is to be inserted
            dto.JobStatus = null;
            dto.Customer = null;
            dto.Vehicle = null;
            dto.AdvisorUser = null;
            dto.ServiceGroup = null;
            dto.EstimatorUser = null;
            dto.ApproverUser = null;

            var createdDto = await base.CreateAsync(dto);

            return createdDto;
        }

        public override async Task<EstimateDTO> UpdateAsync(EstimateDTO dto)
        {
            try
            {
                // Convert everything to uppercase.
                dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

                IMapper mapper = InitializeMapper();

                // Remove FKs. only parent table is to be updated
                dto.JobStatus = null;
                dto.Customer = null;
                dto.Vehicle = null;
                dto.AdvisorUser = null;
                dto.ServiceGroup = null;
                dto.EstimatorUser = null;
                dto.ApproverUser = null;

                var dtoMap = mapper.Map<Estimate>(dto);

                await base.UpdateByEntityAsync(dtoMap);

                return dto;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public async Task<List<EstimateDTO>> GetAllEstimateByCustomerIdAsync(int customerId)
        {
            try
            {
                List<EstimateDTO> dtoList = new List<EstimateDTO>();
                var entityList = await repo.GetAllEstimateByCustomerIdAsync(customerId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<EstimateDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}