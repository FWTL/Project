using System;
using System.Data;
using System.Threading.Tasks;
using FWTL.Database.Access.Entities;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Database.Access
{
    public class AppDatabaseContext : DbContext, IDatabaseContext
    {
        public AppDatabaseContext(DbContextOptions<AppDatabaseContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        public IDbConnection Connection => Database.GetDbConnection();

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public void SaveChangesSync()
        {
            base.SaveChanges();
        }
    }
}