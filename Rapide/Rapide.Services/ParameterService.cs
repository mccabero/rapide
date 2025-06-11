using AutoMapper;
using Castle.Core.Internal;
using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace Rapide.Services
{
    public class ParameterService(IParameterRepo repo) : BaseService<Parameter, ParameterDTO>(repo), IParameterService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<ParameterGroup, ParameterGroupDTO>();

                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroup>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<ParameterDTO>> GetAllParameterAsync()
        {
            try
            {
                List<ParameterDTO> dtoList = new List<ParameterDTO>();
                var entityList = await repo.GetAllParameterAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<ParameterDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ParameterDTO?> GetParameterByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetParameterByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                var mapData = mapper.Map<ParameterDTO>(entity);

                return mapData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ParameterDTO?> GetAsync(Expression<Func<Parameter, bool>> predicate)
        {
            try
            {
                var userDto = await base.GetAsync(predicate);

                if (userDto == null)
                    return null;

                return userDto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ParameterDTO> CreateAsync(ParameterDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.ParameterGroup = null;

            var dtoMap = mapper.Map<Parameter>(dto);

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<ParameterDTO> UpdateAsync(ParameterDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.ParameterGroup = null;

            var dtoMap = mapper.Map<Parameter>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}