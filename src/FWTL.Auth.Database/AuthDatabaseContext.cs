using FWTL.Auth.Database.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using FWTL.Aggregate;
using FWTL.Core.Database;

namespace FWTL.Auth.Database
{
    public class AuthDatabaseContext : IdentityDbContext<User, Role, Guid>, IAuthDatabaseContext
    {
        private readonly AuthDatabaseCredentials _credentials;

        public AuthDatabaseContext(AuthDatabaseCredentials credentials)
        {
            _credentials = credentials;
        }

        public DbSet<TelegramAccount> TelegramAccount { get; set; }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_credentials.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new TelegramAccountConfiguration());
            base.OnModelCreating(builder);
        }

        public void SaveChangesSync()
        {
            base.SaveChanges();
        }
    }
}