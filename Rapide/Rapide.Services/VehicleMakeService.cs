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
    public class VehicleMakeService(IVehicleMakeRepo repo) : BaseService<VehicleMake, VehicleMakeDTO>(repo), IVehicleMakeService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VehicleMake, VehicleMakeDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<ParameterGroup, ParameterGroupDTO>();

                cfg.CreateMap<VehicleMakeDTO, VehicleMake>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroup>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<VehicleMakeDTO>> GetAllAsync()
        {
            try
            {
                List<VehicleMakeDTO> dtoList = new List<VehicleMakeDTO>();
                var entityList = await repo.GetAllAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<VehicleMakeDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<VehicleMakeDTO?> GetAsync(Expression<Func<VehicleMake, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<VehicleMakeDTO?> GetVehicleMakeByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetVehicleMakeByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<VehicleMakeDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<VehicleMakeDTO> CreateAsync(VehicleMakeDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.RegionParameter = null;

            var dtoMap = mapper.Map<VehicleMake>(dto);

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<VehicleMakeDTO> UpdateAsync(VehicleMakeDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.RegionParameter = null;

            var dtoMap = mapper.Map<VehicleMake>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}