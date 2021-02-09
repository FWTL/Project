using FWTL.Core.Database;
using FWTL.Database.Access.Entities;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Database.Access
{
    public interface IDatabaseContext : IDbContext
    {
        DbSet<Account> Accounts { get; set; }
    }
}