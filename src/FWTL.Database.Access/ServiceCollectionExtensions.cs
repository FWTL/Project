using FWTL.Core.Credentials;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FWTL.CurrentUser
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDatabase<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options => options.UseSqlServer(connectionString));
        }
    }
}
