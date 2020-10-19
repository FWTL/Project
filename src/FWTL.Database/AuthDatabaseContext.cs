using System;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.NodaTime.Extensions;
using FWTL.Aggregate;
using FWTL.Database.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Database
{
    public class DatabaseContext : DbContext, Core.Database.DatabaseContext
    {
        private readonly AuthDatabaseCredentials _credentials;

        public DatabaseContext(AuthDatabaseCredentials credentials)
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