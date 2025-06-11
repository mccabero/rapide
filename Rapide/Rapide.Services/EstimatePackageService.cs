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
    public class EstimatePackageService(IEstimatePackageRepo repo) : BaseService<EstimatePackage, EstimatePackageDTO>(repo), IEstimatePackageService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EstimatePackage, EstimatePackageDTO>();
                cfg.CreateMap<Estimate, EstimateDTO>();
                cfg.CreateMap<Package, PackageDTO>();

                cfg.CreateMap<PaymentDTO, Payment>();
                cfg.CreateMap<EstimatePackageDTO, EstimatePackage>();
                cfg.CreateMap<EstimateDTO, Estimate>();
                cfg.CreateMap<PackageDTO, Package>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<EstimatePackageDTO>> GetAllEstimatePackageAsync()
        {
            try
            {
                List<EstimatePackageDTO> dtoList = new List<EstimatePackageDTO>();
                var entityList = await repo.GetAllEstimatePackageAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<EstimatePackageDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<EstimatePackageDTO?> GetAsync(Expression<Func<EstimatePackage, bool>> predicate)
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

        public async Task<EstimatePackageDTO?> GetEstimatePackageByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetEstimatePackageByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<EstimatePackageDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<EstimatePackageDTO>> GetAllEstimatePackageByEstimateIdAsync(int estimateId)
        {
            try
            {
                List<EstimatePackageDTO> dtoList = new List<EstimatePackageDTO>();
                var entityList = await repo.GetAllEstimatePackageByEstimateIdAsync(estimateId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<EstimatePackageDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<EstimatePackageDTO> CreateAsync(EstimatePackageDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Estimate = null;
            dto.Package = null;

            var dtoMap = new Entities.EstimatePackage()
            {
                EstimateId = dto.EstimateId,
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

        public override async Task<EstimatePackageDTO> UpdateAsync(EstimatePackageDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Estimate = null;
            dto.Package = null;

            var dtoMap = mapper.Map<Entities.EstimatePackage>(dto);

            var dataToCheck = await GetByIdAsync(dto.Id);

            if (dataToCheck == null)
            {
                dto.PackageId = dto.PackageId;
                dto.EstimateId = dto.EstimateId;
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