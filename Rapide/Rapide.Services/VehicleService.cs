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
        
        public async Task<List<VehicleDTO>> GetAllVehicleByCustomerIdAsync(int customerId)
        {
            try
            {
                List<VehicleDTO> dtoList = new List<VehicleDTO>();
                var entityList = await repo.GetAllVehicleByCustomerIdAsync(customerId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = MappingHelper.InitializeMapper();
                dtoList = mapper.Map<List<VehicleDTO>>(entityList);

                //IMapper mapper = InitializeMapper();
                //dtoList = mapper.Map<List<VehicleDTO>>(entityList);

                //foreach (var e in entityList)
                //    dtoList.Add(mapper.Map<VehicleDTO>(e));

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

                IMapper mapper = MappingHelper.InitializeMapper();
                dtoList = mapper.Map<List<VehicleDTO>>(entityList);

                //foreach (var e in entityList)
                //    dtoList.Add(mapper.Map<VehicleDTO>(e));

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
                VehicleDTO dto = new VehicleDTO();

                if (entity == null)
                    return null;

                IMapper mapper = MappingHelper.InitializeMapper();
                dto = mapper.Map<VehicleDTO>(entity);

                return dto;
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
                VehicleDTO dto = new VehicleDTO();

                if (entity == null)
                    return null;

                IMapper mapper = MappingHelper.InitializeMapper();
                dto = mapper.Map<VehicleDTO>(entity);

                return dto;
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

            IMapper mapper = MappingHelper.InitializeMapper();

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

            IMapper mapper = MappingHelper.InitializeMapper();

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