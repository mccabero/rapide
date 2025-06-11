using Microsoft.EntityFrameworkCore;
using Rapide.Contracts.Repositories;
using Rapide.Entities;
using Rapide.Repositories.DBContext;
using System;

namespace Rapide.Repositories.Repos
{
    public class UserRepo(IDbContextFactory<RapideDbContext> context) : BaseRepo<User>(context), IUserRepo
    {
        public async Task<List<User>> GetAllUserRoleAsync()
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<User>()
                .Include(x => x.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetUserRoleByEmailAsync(string email)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<User>()
                .Include(x => x.Role)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetUserRoleByIdAsync(int id)
        {
            await using var context = await Factory.CreateDbContextAsync();

            return await context.Set<User>()
                .Include(x => x.Role)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}