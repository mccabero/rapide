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
    public class InspectionTechnicianService(IInspectionTechnicianRepo repo) : BaseService<InspectionTechnician, InspectionTechnicianDTO>(repo), IInspectionTechnicianService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Inspection, InspectionDTO>();
                cfg.CreateMap<InspectionTechnician, InspectionTechnicianDTO>();
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Role, RoleDTO>();

                cfg.CreateMap<InspectionDTO, Inspection>();
                cfg.CreateMap<InspectionTechnicianDTO, InspectionTechnician>();
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<RoleDTO, Role>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<InspectionTechnicianDTO>> GetAllInspectionTechnicianAsync()
        {
            try
            {
                List<InspectionTechnicianDTO> dtoList = new List<InspectionTechnicianDTO>();
                var entityList = await repo.GetAllInspectionTechnicianAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<InspectionTechnicianDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<InspectionTechnicianDTO?> GetAsync(Expression<Func<InspectionTechnician, bool>> predicate)
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

        public async Task<InspectionTechnicianDTO?> GetInspectionTechnicianByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetInspectionTechnicianByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<InspectionTechnicianDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<InspectionTechnicianDTO>> GetAllInspectionTechnicianByInspectionIdAsync(int inspectionId)
        {
            try
            {
                List<InspectionTechnicianDTO> dtoList = new List<InspectionTechnicianDTO>();
                var entityList = await repo.GetAllInspectionTechnicianByInspectionIdAsync(inspectionId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<InspectionTechnicianDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<InspectionTechnicianDTO> CreateAsync(InspectionTechnicianDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.TechnicianUser = null;
            dto.Inspection = null;

            //var dtoMap = mapper.Map<InspectionTechnician>(dto);
            var dtoMap = new Entities.InspectionTechnician()
            {
                InspectionId = dto.InspectionId,
                TechnicianUserId = dto.TechnicianUserId,
                CreatedById = dto.CreatedById,
                CreatedDateTime = dto.CreatedDateTime,
                UpdatedById = dto.UpdatedById,
                UpdatedDateTime = dto.UpdatedDateTime
            };

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<InspectionTechnicianDTO> UpdateAsync(InspectionTechnicianDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.TechnicianUser = null;
            dto.Inspection = null;

            var dtoMap = mapper.Map<InspectionTechnician>(dto);

            var dataToCheck = await GetByIdAsync(dto.Id);

            if (dataToCheck == null)
            {
                dto.InspectionId = dto.InspectionId;
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