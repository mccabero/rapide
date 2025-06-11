using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;

namespace Rapide.Repositories.Repos
{
    public class CompanyInfoRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<CompanyInfo>(context), ICompanyInfoRepo
    {
        public async Task<List<CompanyInfo>> GetAllAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<CompanyInfo>()
                .AsNoTracking()
                .ToListAsync();
        }
    }
}