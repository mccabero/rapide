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
    public class VehicleService(IVehicleRepo repo) : BaseService<Vehicle, VehicleDTO>(repo), IVehicleService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Customer, CustomerDTO>();
                cfg.CreateMap<Vehicle, VehicleDTO>();
                cfg.CreateMap<VehicleMake, VehicleMakeDTO>();
                cfg.CreateMap<VehicleModel, VehicleModelDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<ParameterGroup, ParameterGroupDTO>();

                cfg.CreateMap<CustomerDTO, Customer>();
                cfg.CreateMap<VehicleDTO, Vehicle>();
                cfg.CreateMap<VehicleMakeDTO, VehicleMake>();
                cfg.CreateMap<VehicleModelDTO, VehicleModel>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroup>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<VehicleDTO>> GetAllVehicleByCustomerIdAsync(int customerId)
        {
            try
            {
                List<VehicleDTO> dtoList = new List<VehicleDTO>();
                var entityList = await repo.GetAllVehicleByCustomerIdAsync(customerId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<VehicleDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<VehicleDTO>> GetAllVehicleAsync()
        {
            try
            {
                List<VehicleDTO> dtoList = new List<VehicleDTO>();
                var entityList = await repo.GetAllVehicleAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<VehicleDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<VehicleDTO?> GetAsync(Expression<Func<Vehicle, bool>> predicate)
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

        public async Task<VehicleDTO?> GetVehicleByModelAsync(string model)
        {
            try
            {
                var entity = await repo.GetVehicleByModelAsync(model);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<VehicleDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<VehicleDTO?> GetVehicleByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetVehicleByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<VehicleDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<VehicleDTO> CreateAsync(VehicleDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Customer = null;
            dto.VehicleModel = null;
            dto.TransmissionParameter = null;
            dto.EngineTypeParameter = null;
            dto.EngineSizeParameter = null;
            dto.OdometerParameter = null;
            dto.CustomerRegistrationTypeParameter = null;

            var dtoMap = mapper.Map<Vehicle>(dto);

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<VehicleDTO> UpdateAsync(VehicleDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Customer = null;
            dto.VehicleModel = null;
            dto.TransmissionParameter = null;
            dto.EngineTypeParameter = null;
            dto.EngineSizeParameter = null;
            dto.OdometerParameter = null;
            dto.CustomerRegistrationTypeParameter = null;

            var dtoMap = mapper.Map<Vehicle>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}