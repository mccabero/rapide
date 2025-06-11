using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class ServiceRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<Service>(context), IServiceRepo
    {
        public async Task<List<Service>> GetAllServiceAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Service>()
                .Include(x => x.ServiceGroup)
                .Include(x => x.ServiceCategory)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Service?> GetServiceByCodeAsync(string code)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Service>()
                .Include(x => x.ServiceGroup)
                .Include(x => x.ServiceCategory)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Code == code);
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<Service>()
                .Include(x => x.ServiceGroup)
                .Include(x => x.ServiceCategory)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}