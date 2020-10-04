using EntityFrameworkCore.SqlServer.NodaTime.Extensions;
using FWTL.Aggregate;
using FWTL.Auth.Database.Configuration;
using FWTL.Core.Database;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FWTL.Auth.Database
{
    public class AuthDatabaseContext : IdentityDbContext<User, Role, Guid>, IAuthDatabaseContext
    {
        private readonly AuthDatabaseCredentials _credentials;

        public AuthDatabaseContext(AuthDatabaseCredentials credentials)
        {
            _credentials = credentials;
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<AccountJob> AccountJobs { get; set; }

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
            optionsBuilder.UseSqlServer(_credentials.ConnectionString, options => options.UseNodaTime());
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new TelegramAccountConfiguration());
            builder.ApplyConfiguration(new TelegramAccountJobConfiguration());
            builder.ApplyConfiguration(new JobConfiguration());
            base.OnModelCreating(builder);
        }

        public void SaveChangesSync()
        {
            base.SaveChanges();
        }
    }
}