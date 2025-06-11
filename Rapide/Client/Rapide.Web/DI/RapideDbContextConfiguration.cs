using Microsoft.EntityFrameworkCore;
using Rapide.Repositories.DBContext;

namespace Rapide.Web.DI
{
    public static class RapideDbContextConfiguration
    {
        public static IServiceCollection AddRapideDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            return services
                .AddDbContextFactory<RapideDbContext>(option => option.UseSqlServer(connectionString))
                .AddDbContextFactory<DbContext>(option => {
                    option.UseSqlServer(connectionString);
                    option.EnableSensitiveDataLogging();
                });
        }
    }
}
