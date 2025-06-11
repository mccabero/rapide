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
    public class EstimateTechnicianService(IEstimateTechnicianRepo repo) : BaseService<EstimateTechnician, EstimateTechnicianDTO>(repo), IEstimateTechnicianService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Estimate, EstimateDTO>();
                cfg.CreateMap<EstimateTechnician, EstimateTechnicianDTO>();
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Role, RoleDTO>();

                cfg.CreateMap<EstimateDTO, Estimate>();
                cfg.CreateMap<EstimateTechnicianDTO, EstimateTechnician>();
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<RoleDTO, Role>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<EstimateTechnicianDTO>> GetAllEstimateTechnicianAsync()
        {
            try
            {
                List<EstimateTechnicianDTO> dtoList = new List<EstimateTechnicianDTO>();
                var entityList = await repo.GetAllEstimateTechnicianAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<EstimateTechnicianDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<EstimateTechnicianDTO?> GetAsync(Expression<Func<EstimateTechnician, bool>> predicate)
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

        public async Task<EstimateTechnicianDTO?> GetEstimateTechnicianByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetEstimateTechnicianByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<EstimateTechnicianDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<EstimateTechnicianDTO>> GetAllEstimateTechnicianByEstimateIdAsync(int estimateId)
        {
            try
            {
                List<EstimateTechnicianDTO> dtoList = new List<EstimateTechnicianDTO>();
                var entityList = await repo.GetAllEstimateTechnicianByEstimateIdAsync(estimateId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<EstimateTechnicianDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<EstimateTechnicianDTO> CreateAsync(EstimateTechnicianDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.TechnicianUser = null;
            dto.Estimate = null;

            //var dtoMap = mapper.Map<EstimateTechnician>(dto);
            var dtoMap = new Entities.EstimateTechnician()
            {
                EstimateId = dto.EstimateId,
                TechnicianUserId = dto.TechnicianUserId,
                CreatedById = dto.CreatedById,
                CreatedDateTime = dto.CreatedDateTime,
                UpdatedById = dto.UpdatedById,
                UpdatedDateTime = dto.UpdatedDateTime
            };

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<EstimateTechnicianDTO> UpdateAsync(EstimateTechnicianDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.TechnicianUser = null;
            dto.Estimate = null;

            var dtoMap = mapper.Map<EstimateTechnician>(dto);

            var dataToCheck = await GetByIdAsync(dto.Id);

            if (dataToCheck == null)
            {
                dto.EstimateId = dto.EstimateId;
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