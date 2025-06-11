using AutoMapper;
using Castle.Core.Internal;
using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;
using System.Transactions;

namespace Rapide.Services
{
    public class EstimateProductService(IEstimateProductRepo repo) : BaseService<EstimateProduct, EstimateProductDTO>(repo), IEstimateProductService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Estimate, EstimateDTO>();
                cfg.CreateMap<EstimateProduct, EstimateProductDTO>();
                cfg.CreateMap<Product, ProductDTO>();
                cfg.CreateMap<ProductGroup, ProductGroupDTO>();
                cfg.CreateMap<ProductCategory, ProductCategoryDTO>();
                cfg.CreateMap<UnitOfMeasure, UnitOfMeasureDTO>();
                cfg.CreateMap<Manufacturer, ManufacturerDTO>();
                cfg.CreateMap<Supplier, SupplierDTO>();

                cfg.CreateMap<SupplierDTO, Supplier>();
                cfg.CreateMap<ManufacturerDTO, Manufacturer>();
                cfg.CreateMap<UnitOfMeasureDTO, UnitOfMeasure>();
                cfg.CreateMap<EstimateDTO, Estimate>();
                cfg.CreateMap<EstimateProductDTO, EstimateProduct>();
                cfg.CreateMap<ProductDTO, Product>();
                cfg.CreateMap<ProductGroupDTO, ProductGroup>();
                cfg.CreateMap<ProductCategoryDTO, ProductCategory>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<EstimateProductDTO>> GetAllEstimateProductAsync()
        {
            try
            {
                List<EstimateProductDTO> dtoList = new List<EstimateProductDTO>();
                var entityList = await repo.GetAllEstimateProductAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<EstimateProductDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<EstimateProductDTO?> GetAsync(Expression<Func<EstimateProduct, bool>> predicate)
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

        public async Task<EstimateProductDTO?> GetEstimateProductByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetEstimateProductByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<EstimateProductDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<EstimateProductDTO>> GetAllEstimateProductByEstimateIdAsync(int estimateId)
        {
            try
            {
                List<EstimateProductDTO> dtoList = new List<EstimateProductDTO>();
                var entityList = await repo.GetAllEstimateProductByEstimateIdAsync(estimateId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<EstimateProductDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<EstimateProductDTO> CreateAsync(EstimateProductDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Product = null;
            dto.Estimate = null;

            //var dtoMap = mapper.Map<EstimateProduct>(dto);
            var dtoMap = new Entities.EstimateProduct()
            {
                IsPackage = dto.IsPackage,
                IsRequired = dto.IsRequired,
                PackageId = dto.PackageId,
                EstimateId = dto.EstimateId,
                ProductId = dto.ProductId,
                Price = dto.Price,
                Qty = dto.Qty,
                Amount = dto.Amount,
                IncentiveSA = dto.IncentiveSA,
                IncentiveTech = dto.IncentiveTech,
                CreatedById = dto.CreatedById,
                CreatedDateTime = dto.CreatedDateTime,
                UpdatedById = dto.UpdatedById,
                UpdatedDateTime = dto.UpdatedDateTime
            };

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<EstimateProductDTO> UpdateAsync(EstimateProductDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Product = null;
            dto.Estimate = null;

            var dtoMap = mapper.Map<EstimateProduct>(dto);

            var dataToCheck = await GetByIdAsync(dto.Id);

            if (dataToCheck == null)
            {
                dto.EstimateId = dto.EstimateId;
                dto.PackageId = dto.PackageId;
                dto.IncentiveSA = dto.IncentiveSA;
                dto.IncentiveTech = dto.IncentiveTech;
                dto.UpdatedById = dto.UpdatedById;
                dto.UpdatedDateTime = DateTime.Now;

                await CreateAsync(dto);
            }
            else
            {
                await base.UpdateByEntityAsync(dtoMap);
            }

            return dto;
        }
    }
}