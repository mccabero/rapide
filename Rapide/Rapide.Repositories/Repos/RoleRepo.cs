using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class RoleRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Role>(context), IRoleRepo
    {
        public async Task<List<Role>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context
                .Set<Role>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}