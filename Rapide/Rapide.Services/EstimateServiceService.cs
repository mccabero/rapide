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
    public class EstimateServiceService(IEstimateServiceRepo repo) : BaseService<Entities.EstimateService, EstimateServiceDTO>(repo), IEstimateServiceService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Estimate, EstimateDTO>();
                cfg.CreateMap<Entities.EstimateService, EstimateServiceDTO>();
                cfg.CreateMap<Service, ServiceDTO>();
                cfg.CreateMap<ServiceGroup, ServiceGroupDTO>();
                cfg.CreateMap<ServiceCategory, ServiceCategoryDTO>();
                cfg.CreateMap<UnitOfMeasure, UnitOfMeasureDTO>();
                cfg.CreateMap<Manufacturer, ManufacturerDTO>();
                cfg.CreateMap<Supplier, SupplierDTO>();

                cfg.CreateMap<SupplierDTO, Supplier>();
                cfg.CreateMap<ManufacturerDTO, Manufacturer>();
                cfg.CreateMap<UnitOfMeasureDTO, UnitOfMeasure>();
                cfg.CreateMap<EstimateDTO, Estimate>();
                cfg.CreateMap<EstimateServiceDTO, Entities.EstimateService>();
                cfg.CreateMap<ServiceDTO, Service>();
                cfg.CreateMap<ServiceGroupDTO, ServiceGroup>();
                cfg.CreateMap<ServiceCategoryDTO, ServiceCategory>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<EstimateServiceDTO>> GetAllEstimateServiceAsync()
        {
            try
            {
                List<EstimateServiceDTO> dtoList = new List<EstimateServiceDTO>();
                var entityList = await repo.GetAllEstimateServiceAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<EstimateServiceDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<EstimateServiceDTO?> GetAsync(Expression<Func<Entities.EstimateService, bool>> predicate)
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

        public async Task<EstimateServiceDTO?> GetEstimateServiceByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetEstimateServiceByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<EstimateServiceDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<EstimateServiceDTO>> GetAllEstimateServiceByEstimateIdAsync(int estimateId)
        {
            try
            {
                List<EstimateServiceDTO> dtoList = new List<EstimateServiceDTO>();
                var entityList = await repo.GetAllEstimateServiceByEstimateIdAsync(estimateId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<EstimateServiceDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<EstimateServiceDTO> CreateAsync(EstimateServiceDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Estimate = null;
            dto.Service = null;

            var dtoMap = new Entities.EstimateService()
            {
                IsPackage = dto.IsPackage,
                IsRequired = dto.IsRequired,
                PackageId = dto.PackageId,
                EstimateId = dto.EstimateId,
                ServiceId = dto.ServiceId,
                Rate = dto.Rate,
                Hours = dto.Hours,
                Amount = dto.Amount,
                CreatedById = dto.CreatedById,
                CreatedDateTime = dto.CreatedDateTime,
                UpdatedById = dto.UpdatedById,
                UpdatedDateTime = dto.UpdatedDateTime
            };

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<EstimateServiceDTO> UpdateAsync(EstimateServiceDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Estimate = null;
            dto.Service = null;

            var dtoMap = mapper.Map<Entities.EstimateService>(dto);

            var dataToCheck = await GetByIdAsync(dto.Id);

            if (dataToCheck == null)
            {
                dto.EstimateId = dto.EstimateId;
                dto.PackageId = dto.PackageId;
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