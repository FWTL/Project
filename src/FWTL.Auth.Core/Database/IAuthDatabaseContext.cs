using FWTL.Aggragate;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Core.Database
{
    public interface IAuthDatabaseContext : IDbContext
    {
        DbSet<TelegramAccount> TelegramAccount { get; set; }
    }
}