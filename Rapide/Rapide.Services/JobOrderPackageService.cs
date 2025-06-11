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
    public class JobOrderPackageService(IJobOrderPackageRepo repo) : BaseService<JobOrderPackage, JobOrderPackageDTO>(repo), IJobOrderPackageService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JobOrderPackage, JobOrderPackageDTO>();
                cfg.CreateMap<JobOrder, JobOrderDTO>();
                cfg.CreateMap<Package, PackageDTO>();

                cfg.CreateMap<PaymentDTO, Payment>();
                cfg.CreateMap<JobOrderPackageDTO, JobOrderPackage>();
                cfg.CreateMap<JobOrderDTO, JobOrder>();
                cfg.CreateMap<PackageDTO, Package>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<JobOrderPackageDTO>> GetAllJobOrderPackageAsync()
        {
            try
            {
                List<JobOrderPackageDTO> dtoList = new List<JobOrderPackageDTO>();
                var entityList = await repo.GetAllJobOrderPackageAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<JobOrderPackageDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<JobOrderPackageDTO?> GetAsync(Expression<Func<JobOrderPackage, bool>> predicate)
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

        public async Task<JobOrderPackageDTO?> GetJobOrderPackageByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetJobOrderPackageByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<JobOrderPackageDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<JobOrderPackageDTO>> GetAllJobOrderPackageByJobOrderIdAsync(int jobOrderId)
        {
            try
            {
                List<JobOrderPackageDTO> dtoList = new List<JobOrderPackageDTO>();
                var entityList = await repo.GetAllJobOrderPackageByJobOrderIdAsync(jobOrderId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<JobOrderPackageDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<JobOrderPackageDTO> CreateAsync(JobOrderPackageDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.JobOrder = null;
            dto.Package = null;

            var dtoMap = new Entities.JobOrderPackage()
            {
                JobOrderId = dto.JobOrderId,
                PackageId = dto.PackageId,
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

        public override async Task<JobOrderPackageDTO> UpdateAsync(JobOrderPackageDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.JobOrder = null;
            dto.Package = null;

            var dtoMap = mapper.Map<Entities.JobOrderPackage>(dto);

            var dataToCheck = await GetByIdAsync(dto.Id);

            if (dataToCheck == null)
            {
                dto.PackageId = dto.PackageId;
                dto.JobOrderId = dto.JobOrderId;
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