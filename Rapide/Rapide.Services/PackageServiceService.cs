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
    public class PackageServiceService(IPackageServiceRepo repo) : BaseService<Entities.PackageService, PackageServiceDTO>(repo), IPackageServiceService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Package, PackageDTO>();
                cfg.CreateMap<Entities.PackageService, PackageServiceDTO>();
                cfg.CreateMap<Service, ServiceDTO>();
                cfg.CreateMap<ServiceGroup, ServiceGroupDTO>();
                cfg.CreateMap<ServiceCategory, ServiceCategoryDTO>();
                cfg.CreateMap<UnitOfMeasure, UnitOfMeasureDTO>();
                cfg.CreateMap<Manufacturer, ManufacturerDTO>();
                cfg.CreateMap<Supplier, SupplierDTO>();

                cfg.CreateMap<SupplierDTO, Supplier>();
                cfg.CreateMap<ManufacturerDTO, Manufacturer>();
                cfg.CreateMap<UnitOfMeasureDTO, UnitOfMeasure>();
                cfg.CreateMap<PackageDTO, Package>();
                cfg.CreateMap<PackageServiceDTO, Entities.PackageService>();
                cfg.CreateMap<ServiceDTO, Service>();
                cfg.CreateMap<ServiceGroupDTO, ServiceGroup>();
                cfg.CreateMap<ServiceCategoryDTO, ServiceCategory>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<PackageServiceDTO>> GetAllPackageServiceByPackageIdAsync(int packageId)
        {
            try
            {
                List<PackageServiceDTO> dtoList = new List<PackageServiceDTO>();
                var entityList = await repo.GetAllPackageServiceByPackageIdAsync(packageId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<PackageServiceDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<PackageServiceDTO>> GetAllPackageServiceAsync()
        {
            try
            {
                List<PackageServiceDTO> dtoList = new List<PackageServiceDTO>();
                var entityList = await repo.GetAllPackageServiceAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<PackageServiceDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PackageServiceDTO?> GetPackageServiceByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetPackageServiceByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<PackageServiceDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<PackageServiceDTO?> GetAsync(Expression<Func<Entities.PackageService, bool>> predicate)
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

        public override async Task<PackageServiceDTO> CreateAsync(PackageServiceDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Package = null;
            dto.Service = null;

            var dtoMap = mapper.Map<Entities.PackageService>(dto);

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<PackageServiceDTO> UpdateAsync(PackageServiceDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Package = null;
            dto.Service = null;

            var dtoMap = mapper.Map<Entities.PackageService>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}