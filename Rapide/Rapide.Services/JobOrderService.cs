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
    public class JobOrderService(IJobOrderRepo repo) : BaseService<JobOrder, JobOrderDTO>(repo), IJobOrderService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JobOrder, JobOrderDTO>();
                cfg.CreateMap<JobStatus, JobStatusDTO>();
                cfg.CreateMap<Estimate, EstimateDTO>();
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
                cfg.CreateMap<JobOrderDTO, JobOrder>();
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

        public async Task<List<JobOrderDTO>> GetAllJobOrderAsync()
        {
            try
            {
                List<JobOrderDTO> dtoList = new List<JobOrderDTO>();
                var entityList = await repo.GetAllJobOrderAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<JobOrderDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<JobOrderDTO>> GetAllJobOrderByCustomerIdAsync(int customerId)
        {
            try
            {
                List<JobOrderDTO> dtoList = new List<JobOrderDTO>();
                var entityList = await repo.GetAllJobOrderByCustomerIdAsync(customerId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<JobOrderDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<JobOrderDTO>> GetAllJobOrderByVehicleIdAsync(int vehicleId)
        {
            try
            {
                List<JobOrderDTO> dtoList = new List<JobOrderDTO>();
                var entityList = await repo.GetAllJobOrderByVehicleIdAsync(vehicleId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<JobOrderDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<JobOrderDTO?> GetAsync(Expression<Func<JobOrder, bool>> predicate)
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

        public async Task<JobOrderDTO?> GetJobOrderByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetJobOrderByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<JobOrderDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<JobOrderDTO> CreateAsync(JobOrderDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
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

        public override async Task<JobOrderDTO> UpdateAsync(JobOrderDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.JobStatus = null;
            dto.Customer = null;
            dto.Vehicle = null;
            dto.AdvisorUser = null;
            dto.ServiceGroup = null;
            dto.EstimatorUser = null;
            dto.ApproverUser = null;

            var dtoMap = mapper.Map<JobOrder>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}