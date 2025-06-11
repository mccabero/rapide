using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class CustomerRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Customer>(context), ICustomerRepo
    {
        public async Task<List<Customer>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Customer>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}