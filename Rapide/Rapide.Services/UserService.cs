using AutoMapper;
using Castle.Core.Internal;
using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace Rapide.Services
{
    public class UserService(IUserRepo repo) : BaseService<User, UserDTO>(repo), IUserService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Role, RoleDTO>();
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<RoleDTO, Role>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<UserDTO>> GetAllUserRoleAsync()
        {
            try
            {
                List<UserDTO> dtoList = new List<UserDTO>();
                var entityList = await repo.GetAllUserRoleAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<UserDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public override async Task<UserDTO?> GetAsync(Expression<Func<User, bool>> predicate)
        {
            try
            {
                var userDto = await base.GetAsync(predicate);

                if (userDto == null)
                    return null;

                return userDto;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<UserDTO?> GetUserRoleByEmailAsync(string email)
        {
            try
            {
                var entity = await repo.GetUserRoleByEmailAsync(email);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<UserDTO>(entity);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<UserDTO?> GetUserRoleByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetUserRoleByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<UserDTO>(entity);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public override async Task<UserDTO> CreateAsync(UserDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Role = null;

            var createdDto = await base.CreateAsync(dto);

            return createdDto;
        }

        public override async Task<UserDTO> UpdateAsync(UserDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Role = null;

            var dtoMap = mapper.Map<User>(dto);
            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}
