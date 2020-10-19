using FWTL.Aggregate;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Core.Database
{
    public interface DatabaseContext : IDbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<AccountJob> AccountJobs { get; set; }
    }
}