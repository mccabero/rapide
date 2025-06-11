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
    public class QuickSalesProductService(IQuickSalesProductRepo repo) : BaseService<QuickSalesProduct, QuickSalesProductDTO>(repo), IQuickSalesProductService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<QuickSales, QuickSalesDTO>();
                cfg.CreateMap<QuickSalesProduct, QuickSalesProductDTO>();
                cfg.CreateMap<Product, ProductDTO>();
                cfg.CreateMap<ProductGroup, ProductGroupDTO>();
                cfg.CreateMap<ProductCategory, ProductCategoryDTO>();
                cfg.CreateMap<UnitOfMeasure, UnitOfMeasureDTO>();
                cfg.CreateMap<Manufacturer, ManufacturerDTO>();
                cfg.CreateMap<Supplier, SupplierDTO>();

                cfg.CreateMap<SupplierDTO, Supplier>();
                cfg.CreateMap<ManufacturerDTO, Manufacturer>();
                cfg.CreateMap<UnitOfMeasureDTO, UnitOfMeasure>();
                cfg.CreateMap<QuickSalesDTO, QuickSales>();
                cfg.CreateMap<QuickSalesProductDTO, QuickSalesProduct>();
                cfg.CreateMap<ProductDTO, Product>();
                cfg.CreateMap<ProductGroupDTO, ProductGroup>();
                cfg.CreateMap<ProductCategoryDTO, ProductCategory>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<QuickSalesProductDTO>> GetAllQuickSalesProductAsync()
        {
            try
            {
                List<QuickSalesProductDTO> dtoList = new List<QuickSalesProductDTO>();
                var entityList = await repo.GetAllQuickSalesProductAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<QuickSalesProductDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<QuickSalesProductDTO?> GetAsync(Expression<Func<QuickSalesProduct, bool>> predicate)
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

        public async Task<QuickSalesProductDTO?> GetQuickSalesProductByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetQuickSalesProductByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<QuickSalesProductDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<QuickSalesProductDTO>> GetAllQuickSalesProductByQuickSalesIdAsync(int quickSalesId)
        {
            try
            {
                List<QuickSalesProductDTO> dtoList = new List<QuickSalesProductDTO>();
                var entityList = await repo.GetAllQuickSalesProductByQuickSalesIdAsync(quickSalesId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<QuickSalesProductDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<QuickSalesProductDTO> CreateAsync(QuickSalesProductDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Product = null;
            dto.QuickSales = null;

            //var dtoMap = mapper.Map<QuickSalesProduct>(dto);
            var dtoMap = new Entities.QuickSalesProduct()
            {
                QuickSalesId = dto.QuickSalesId,
                ProductId = dto.ProductId,
                Price = dto.Price,
                Qty = dto.Qty,
                Amount = dto.Amount,
                CreatedById = dto.CreatedById,
                CreatedDateTime = dto.CreatedDateTime,
                UpdatedById = dto.UpdatedById,
                UpdatedDateTime = dto.UpdatedDateTime
            };

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<QuickSalesProductDTO> UpdateAsync(QuickSalesProductDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Product = null;
            dto.QuickSales = null;

            var dtoMap = mapper.Map<QuickSalesProduct>(dto);

            var dataToCheck = await GetByIdAsync(dto.Id);

            if (dataToCheck == null)
            {
                dto.QuickSalesId = dto.QuickSalesId;
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