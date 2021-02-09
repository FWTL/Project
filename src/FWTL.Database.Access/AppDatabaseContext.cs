using System;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.NodaTime.Extensions;
using FWTL.Database.Access.Entities;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Database.Access
{
    public class AppDatabaseContext : DbContext, IDatabaseContext
    {
        private readonly AppDatabaseCredentials _credentials;

        public AppDatabaseContext(AppDatabaseCredentials credentials)
        {
            _credentials = credentials;
        }

        public DbSet<Account> Accounts { get; set; }

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
            base.OnModelCreating(builder);
        }

        public void SaveChangesSync()
        {
            base.SaveChanges();
        }
    }
}