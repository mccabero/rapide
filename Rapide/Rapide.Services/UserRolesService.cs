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
    public class UserRolesService(IUserRolesRepo repo) : BaseService<UserRoles, UserRolesDTO>(repo), IUserRolesService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Role, RoleDTO>();
                cfg.CreateMap<UserRoles, UserRolesDTO>();

                cfg.CreateMap<RoleDTO, Role>();
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<UserRolesDTO, UserRoles>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<UserRolesDTO>> GetAllUserRolesAsync()
        {
            try
            {
                List<UserRolesDTO> dtoList = new List<UserRolesDTO>();
                var entityList = await repo.GetAllUserRolesAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<UserRolesDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<UserRolesDTO>> GetUserRolesByUserIdAsync(int userId)
        {
            try
            {
                List<UserRolesDTO> dtoList = new List<UserRolesDTO>();
                var entityList = await repo.GetUserRolesByUserIdAsync(userId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<UserRolesDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<UserRolesDTO?> GetAsync(Expression<Func<UserRoles, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<UserRolesDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}