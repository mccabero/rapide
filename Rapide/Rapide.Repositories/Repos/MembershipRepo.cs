using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class MembershipRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Membership>(context), IMembershipRepo
    {
        public async Task<List<Membership>> GetAllMembershipAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Membership>()
                .Include(x => x.Customer)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
