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
    public class PettyCashService(IPettyCashRepo repo) : BaseService<PettyCash, PettyCashDTO>(repo), IPettyCashService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PettyCash, PettyCashDTO>();
                cfg.CreateMap<JobStatus, JobStatusDTO>();
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<ParameterGroup, ParameterGroupDTO>();
                cfg.CreateMap<Role, RoleDTO>();

                cfg.CreateMap<RoleDTO, Role>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroup>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<PettyCashDTO, PettyCash>();
                cfg.CreateMap<JobStatusDTO, JobStatus>();
                cfg.CreateMap<UserDTO, User>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<PettyCashDTO>> GetAllPettyCashAsync()
        {
            try
            {
                List<PettyCashDTO> dtoList = new List<PettyCashDTO>();
                var entityList = await repo.GetAllPettyCashAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<PettyCashDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<PettyCashDTO?> GetAsync(Expression<Func<PettyCash, bool>> predicate)
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

        public async Task<PettyCashDTO?> GetPettyCashByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetPettyCashByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<PettyCashDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<PettyCashDTO> CreateAsync(PettyCashDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.ApprovedByUser = null;
            dto.PaidByUser = null;
            dto.JobStatus = null;

            var createdDto = await base.CreateAsync(dto);

            return createdDto;
        }

        public override async Task<PettyCashDTO> UpdateAsync(PettyCashDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.ApprovedByUser = null;
            dto.PaidByUser = null;
            dto.JobStatus = null;

            var dtoMap = mapper.Map<PettyCash>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }

        public Task<PettyCashDetailsDTO?> GetPettyCashDetailsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PettyCashDetailsDTO>> GetAllPettyCashDetailsAsync()
        {
            throw new NotImplementedException();
        }
    }
}