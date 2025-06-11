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
    public class ProductService(IProductRepo repo) : BaseService<Product, ProductDTO>(repo), IProductService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductDTO>();
                cfg.CreateMap<ProductGroup, ProductGroupDTO>();
                cfg.CreateMap<ProductCategory, ProductCategoryDTO>();
                cfg.CreateMap<UnitOfMeasure, UnitOfMeasureDTO>();
                cfg.CreateMap<Manufacturer, ManufacturerDTO>();
                cfg.CreateMap<Supplier, SupplierDTO>();

                cfg.CreateMap<ProductDTO, Product>();
                cfg.CreateMap<ProductGroupDTO, ProductGroup>();
                cfg.CreateMap<ProductCategoryDTO, ProductCategory>();
                cfg.CreateMap<UnitOfMeasureDTO, UnitOfMeasure>();
                cfg.CreateMap<ManufacturerDTO, Manufacturer>();
                cfg.CreateMap<SupplierDTO, Supplier>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<ProductDTO>> GetAllProductAsync()
        {
            try
            {
                List<ProductDTO> dtoList = new List<ProductDTO>();
                var entityList = await repo.GetAllProductAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<ProductDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ProductDTO?> GetAsync(Expression<Func<Product, bool>> predicate)
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

        public async Task<ProductDTO?> GetProductByPartNoAsync(string partNo)
        {
            try
            {
                var entity = await repo.GetProductByPartNoAsync(partNo);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<ProductDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetProductByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<ProductDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ProductDTO> CreateAsync(ProductDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.ProductGroup = null;
            dto.ProductCategory = null;
            dto.UnitOfMeasure = null;
            dto.Manufacturer = null;
            dto.Supplier = null;

            var dtoMap = mapper.Map<Product>(dto);

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<ProductDTO> UpdateAsync(ProductDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.ProductGroup = null;
            dto.ProductCategory = null;
            dto.UnitOfMeasure = null;
            dto.Manufacturer = null;
            dto.Supplier = null;

            var dtoMap = mapper.Map<Product>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}