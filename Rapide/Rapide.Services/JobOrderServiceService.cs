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
    public class JobOrderServiceService(IJobOrderServiceRepo repo) : BaseService<Entities.JobOrderService, JobOrderServiceDTO>(repo), IJobOrderServiceService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JobOrder, JobOrderDTO>();
                cfg.CreateMap<Entities.JobOrderService, JobOrderServiceDTO>();
                cfg.CreateMap<Service, ServiceDTO>();
                cfg.CreateMap<ServiceGroup, ServiceGroupDTO>();
                cfg.CreateMap<ServiceCategory, ServiceCategoryDTO>();
                cfg.CreateMap<UnitOfMeasure, UnitOfMeasureDTO>();
                cfg.CreateMap<Manufacturer, ManufacturerDTO>();
                cfg.CreateMap<Supplier, SupplierDTO>();

                cfg.CreateMap<SupplierDTO, Supplier>();
                cfg.CreateMap<ManufacturerDTO, Manufacturer>();
                cfg.CreateMap<UnitOfMeasureDTO, UnitOfMeasure>();
                cfg.CreateMap<JobOrderDTO, JobOrder>();
                cfg.CreateMap<JobOrderServiceDTO, Entities.JobOrderService>();
                cfg.CreateMap<ServiceDTO, Service>();
                cfg.CreateMap<ServiceGroupDTO, ServiceGroup>();
                cfg.CreateMap<ServiceCategoryDTO, ServiceCategory>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<JobOrderServiceDTO>> GetAllJobOrderServiceAsync()
        {
            try
            {
                List<JobOrderServiceDTO> dtoList = new List<JobOrderServiceDTO>();
                var entityList = await repo.GetAllJobOrderServiceAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<JobOrderServiceDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<JobOrderServiceDTO?> GetAsync(Expression<Func<Entities.JobOrderService, bool>> predicate)
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

        public async Task<JobOrderServiceDTO?> GetJobOrderServiceByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetJobOrderServiceByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<JobOrderServiceDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<JobOrderServiceDTO>> GetAllJobOrderServiceByJobOrderIdAsync(int jobOrderId)
        {
            try
            {
                List<JobOrderServiceDTO> dtoList = new List<JobOrderServiceDTO>();
                var entityList = await repo.GetAllJobOrderServiceByJobOrderIdAsync(jobOrderId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<JobOrderServiceDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<JobOrderServiceDTO> CreateAsync(JobOrderServiceDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Service = null;
            dto.JobOrder = null;

            var dtoMap = mapper.Map<Entities.JobOrderService>(dto);

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<JobOrderServiceDTO> UpdateAsync(JobOrderServiceDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Service = null;
            dto.JobOrder = null;

            var dtoMap = mapper.Map<Entities.JobOrderService>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}