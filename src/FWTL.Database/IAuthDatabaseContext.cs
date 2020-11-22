using FWTL.Core.Database;
using FWTL.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Database
{
    public interface IDatabaseContext : IDbContext
    {
        public DbSet<Account> Accounts { get; set; }
    }
}