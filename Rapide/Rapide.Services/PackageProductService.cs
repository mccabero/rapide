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
    public class PackageProductService(IPackageProductRepo repo) : BaseService<Entities.PackageProduct, PackageProductDTO>(repo), IPackageProductService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Package, PackageDTO>();
                cfg.CreateMap<PackageProduct, PackageProductDTO>();
                cfg.CreateMap<Product, ProductDTO>();
                cfg.CreateMap<ProductGroup, ProductGroupDTO>();
                cfg.CreateMap<ProductCategory, ProductCategoryDTO>();
                cfg.CreateMap<UnitOfMeasure, UnitOfMeasureDTO>();
                cfg.CreateMap<Manufacturer, ManufacturerDTO>();
                cfg.CreateMap<Supplier, SupplierDTO>();

                cfg.CreateMap<SupplierDTO, Supplier>();
                cfg.CreateMap<ManufacturerDTO, Manufacturer>();
                cfg.CreateMap<UnitOfMeasureDTO, UnitOfMeasure>();
                cfg.CreateMap<PackageDTO, Package>();
                cfg.CreateMap<PackageProductDTO, PackageProduct>();
                cfg.CreateMap<ProductDTO, Product>();
                cfg.CreateMap<ProductGroupDTO, ProductGroup>();
                cfg.CreateMap<ProductCategoryDTO, ProductCategory>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<PackageProductDTO>> GetAllPackageProductByPackageIdAsync(int packageId)
        {
            try
            {
                List<PackageProductDTO> dtoList = new List<PackageProductDTO>();
                var entityList = await repo.GetAllPackageProductByPackageIdAsync(packageId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<PackageProductDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<PackageProductDTO>> GetAllPackageProductAsync()
        {
            try
            {
                List<PackageProductDTO> dtoList = new List<PackageProductDTO>();
                var entityList = await repo.GetAllPackageProductAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<PackageProductDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PackageProductDTO?> GetPackageProductByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetPackageProductByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<PackageProductDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<PackageProductDTO?> GetAsync(Expression<Func<Entities.PackageProduct, bool>> predicate)
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

        public override async Task<PackageProductDTO> CreateAsync(PackageProductDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Product = null;
            dto.Package = null;

            var dtoMap = mapper.Map<PackageProduct>(dto);

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<PackageProductDTO> UpdateAsync(PackageProductDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Product = null;
            dto.Package = null;

            var dtoMap = mapper.Map<PackageProduct>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}