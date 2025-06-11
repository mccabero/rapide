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
    public class VehicleModelService(IVehicleModelRepo repo) : BaseService<VehicleModel, VehicleModelDTO>(repo), IVehicleModelService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VehicleModel, VehicleModelDTO>();
                cfg.CreateMap<VehicleMake, VehicleMakeDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<ParameterGroup, ParameterGroupDTO>();

                cfg.CreateMap<VehicleModelDTO, VehicleModel>();
                cfg.CreateMap<VehicleMakeDTO, VehicleMake>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroup>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<VehicleModelDTO>> GetAllVehicleModelAsync()
        {
            try
            {
                List<VehicleModelDTO> dtoList = new List<VehicleModelDTO>();
                var entityList = await repo.GetAllVehicleModelAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<VehicleModelDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<VehicleModelDTO?> GetAsync(Expression<Func<VehicleModel, bool>> predicate)
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

        public async Task<VehicleModelDTO?> GetVehicleModelByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetVehicleModelByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<VehicleModelDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<VehicleModelDTO> CreateAsync(VehicleModelDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.VehicleMake = null;
            dto.BodyParameter = null;
            dto.ClassificationParameter = null;

            var dtoMap = mapper.Map<VehicleModel>(dto);

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<VehicleModelDTO> UpdateAsync(VehicleModelDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.VehicleMake = null;
            dto.BodyParameter = null;
            dto.ClassificationParameter = null;

            var dtoMap = mapper.Map<VehicleModel>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}