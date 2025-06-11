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
    public class JobOrderTechnicianService(IJobOrderTechnicianRepo repo) : BaseService<JobOrderTechnician, JobOrderTechnicianDTO>(repo), IJobOrderTechnicianService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JobOrder, JobOrderDTO>();
                cfg.CreateMap<JobOrderTechnician, JobOrderTechnicianDTO>();
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Role, RoleDTO>();

                cfg.CreateMap<JobOrderDTO, JobOrder>();
                cfg.CreateMap<JobOrderTechnicianDTO, JobOrderTechnician>();
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<RoleDTO, Role>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<JobOrderTechnicianDTO>> GetAllJobOrderTechnicianAsync()
        {
            try
            {
                List<JobOrderTechnicianDTO> dtoList = new List<JobOrderTechnicianDTO>();
                var entityList = await repo.GetAllJobOrderTechnicianAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<JobOrderTechnicianDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<JobOrderTechnicianDTO?> GetAsync(Expression<Func<JobOrderTechnician, bool>> predicate)
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

        public async Task<JobOrderTechnicianDTO?> GetJobOrderTechnicianByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetJobOrderTechnicianByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<JobOrderTechnicianDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<JobOrderTechnicianDTO>> GetAllJobOrderTechnicianByJobOrderIdAsync(int estimateId)
        {
            try
            {
                List<JobOrderTechnicianDTO> dtoList = new List<JobOrderTechnicianDTO>();
                var entityList = await repo.GetAllJobOrderTechnicianByJobOrderIdAsync(estimateId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<JobOrderTechnicianDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<JobOrderTechnicianDTO> CreateAsync(JobOrderTechnicianDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.TechnicianUser = null;
            dto.JobOrder = null;

            //var dtoMap = mapper.Map<JobOrderTechnician>(dto);
            var dtoMap = new Entities.JobOrderTechnician()
            {
                JobOrderId = dto.JobOrderId,
                TechnicianUserId = dto.TechnicianUserId,
                CreatedById = dto.CreatedById,
                CreatedDateTime = dto.CreatedDateTime,
                UpdatedById = dto.UpdatedById,
                UpdatedDateTime = dto.UpdatedDateTime
            };

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<JobOrderTechnicianDTO> UpdateAsync(JobOrderTechnicianDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.TechnicianUser = null;
            dto.JobOrder = null;

            var dtoMap = mapper.Map<JobOrderTechnician>(dto);

            var dataToCheck = await GetByIdAsync(dto.Id);

            if (dataToCheck == null)
            {
                dto.JobOrderId = dto.JobOrderId;
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