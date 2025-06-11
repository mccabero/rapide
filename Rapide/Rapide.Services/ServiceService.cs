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
    public class ServiceService(IServiceRepo repo) : BaseService<Service, ServiceDTO>(repo), IServiceService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Service, ServiceDTO>();
                cfg.CreateMap<ServiceCategory, ServiceCategoryDTO>();
                cfg.CreateMap<ServiceGroup, ServiceGroupDTO>();

                cfg.CreateMap<ServiceDTO, Service>();
                cfg.CreateMap<ServiceCategoryDTO, ServiceCategory>();
                cfg.CreateMap<ServiceGroupDTO, ServiceGroup>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<ServiceDTO>> GetAllServiceAsync()
        {
            try
            {
                List<ServiceDTO> dtoList = new List<ServiceDTO>();
                var entityList = await repo.GetAllServiceAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<ServiceDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ServiceDTO?> GetAsync(Expression<Func<Service, bool>> predicate)
        {
            try
            {
                var ServiceDTO = await base.GetAsync(predicate);

                if (ServiceDTO == null)
                    return null;

                return ServiceDTO;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ServiceDTO?> GetServiceByCodeAsync(string code)
        {
            try
            {
                var entity = await repo.GetServiceByCodeAsync(code);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<ServiceDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ServiceDTO?> GetServiceByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetServiceByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<ServiceDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ServiceDTO> CreateAsync(ServiceDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.ServiceCategory = null;
            dto.ServiceGroup = null;

            var dtoMap = mapper.Map<Service>(dto);

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<ServiceDTO> UpdateAsync(ServiceDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.ServiceCategory = null;
            dto.ServiceGroup = null;

            var dtoMap = mapper.Map<Service>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}