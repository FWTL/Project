using System.Threading.Tasks;
using FWTL.Auth.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Auth.Database
{
    public class AuthDatabaseContext : IdentityDbContext<User, Role, long>
    {
        private readonly AuthDatabaseCredentials _credentials;
        private readonly SeedData _seed;

        public AuthDatabaseContext(AuthDatabaseCredentials credentials, SeedData seed)
        {
            _credentials = credentials;
            _seed = seed;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_credentials.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Task task = Task.Run(async () => await _seed.UpdateAsync());
            task.Wait();
            base.OnModelCreating(modelBuilder);
        }
    }
}