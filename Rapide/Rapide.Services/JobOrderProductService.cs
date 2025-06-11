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
    public class JobOrderProductService(IJobOrderProductRepo repo) : BaseService<JobOrderProduct, JobOrderProductDTO>(repo), IJobOrderProductService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JobOrder, JobOrderDTO>();
                cfg.CreateMap<JobOrderProduct, JobOrderProductDTO>();
                cfg.CreateMap<Product, ProductDTO>();
                cfg.CreateMap<ProductGroup, ProductGroupDTO>();
                cfg.CreateMap<ProductCategory, ProductCategoryDTO>();
                cfg.CreateMap<UnitOfMeasure, UnitOfMeasureDTO>();
                cfg.CreateMap<Manufacturer, ManufacturerDTO>();
                cfg.CreateMap<Supplier, SupplierDTO>();

                cfg.CreateMap<SupplierDTO, Supplier>();
                cfg.CreateMap<ManufacturerDTO, Manufacturer>();
                cfg.CreateMap<UnitOfMeasureDTO, UnitOfMeasure>();
                cfg.CreateMap<JobOrderDTO, JobOrder>();
                cfg.CreateMap<JobOrderProductDTO, JobOrderProduct>();
                cfg.CreateMap<ProductDTO, Product>();
                cfg.CreateMap<ProductGroupDTO, ProductGroup>();
                cfg.CreateMap<ProductCategoryDTO, ProductCategory>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<JobOrderProductDTO>> GetAllJobOrderProductAsync()
        {
            try
            {
                List<JobOrderProductDTO> dtoList = new List<JobOrderProductDTO>();
                var entityList = await repo.GetAllJobOrderProductAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<JobOrderProductDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<JobOrderProductDTO?> GetAsync(Expression<Func<JobOrderProduct, bool>> predicate)
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

        public async Task<List<JobOrderProductDTO>> GetAllJobOrderProductByJobOrderIdAsync(int jobOrderId)
        {
            try
            {
                List<JobOrderProductDTO> dtoList = new List<JobOrderProductDTO>();
                var entityList = await repo.GetAllJobOrderProductByJobOrderIdAsync(jobOrderId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<JobOrderProductDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<JobOrderProductDTO?> GetJobOrderProductByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetJobOrderProductByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<JobOrderProductDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<JobOrderProductDTO> CreateAsync(JobOrderProductDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Product = null;
            dto.JobOrder = null;

            var dtoMap = mapper.Map<JobOrderProduct>(dto);

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<JobOrderProductDTO> UpdateAsync(JobOrderProductDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Product = null;
            dto.JobOrder = null;

            var dtoMap = mapper.Map<JobOrderProduct>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}