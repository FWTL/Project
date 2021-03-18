using System;
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