using FWTL.Aggregate;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Core.Database
{
    public interface IAuthDatabaseContext : IDbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<TelegramAccountJob> TelegramAccountJobs { get; set; }
    }
}