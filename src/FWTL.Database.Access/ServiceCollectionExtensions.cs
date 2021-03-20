using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.Database.Access
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDatabase<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IDatabaseContext, AppDatabaseContext>();
        }
    }
}
