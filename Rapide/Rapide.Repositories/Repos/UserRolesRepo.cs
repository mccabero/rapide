using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class UserRolesRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<UserRoles>(context), IUserRolesRepo
    {
        public async Task<List<UserRoles>> GetAllUserRolesAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<UserRoles>()
                    .Include(x => x.User)
                    .Include(x => x.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<UserRoles>> GetUserRolesByUserIdAsync(int userId)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<UserRoles>()
                    .Include(x => x.User)
                    .Include(x => x.Role)
                .Where(x => x.User.Id == userId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}