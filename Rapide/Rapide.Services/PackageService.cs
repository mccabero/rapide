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
    public class PackageService(IPackageRepo repo) : BaseService<Package, PackageDTO>(repo), IPackageService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Package, PackageDTO>();
                cfg.CreateMap<PackageService, PackageServiceDTO>();
                cfg.CreateMap<PackageProduct, PackageProductDTO>();

                cfg.CreateMap<PackageDTO, Package>();
                cfg.CreateMap<PackageServiceDTO, PackageService>();
                cfg.CreateMap<PackageProductDTO, PackageProduct>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<PackageDTO>> GetAllPackageAsync()
        {
            try
            {
                List<PackageDTO> dtoList = new List<PackageDTO>();
                var entityList = await repo.GetAllPackageAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<PackageDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<PackageDTO?> GetAsync(Expression<Func<Package, bool>> predicate)
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

        public async Task<PackageDTO?> GetPackageByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetPackageByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<PackageDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<PackageDTO> CreateAsync(PackageDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted

            var createdDto = await base.CreateAsync(dto);

            return createdDto;
        }

        public override async Task<PackageDTO> UpdateAsync(PackageDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            var dtoMap = mapper.Map<Package>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}