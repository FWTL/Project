using System.Threading.Tasks;

namespace FWTL.Core.Database
{
    public interface IDbContext
    {
        void BeginTransaction();

        void RollbackTransaction();

        void SaveChangesSync();

        Task SaveChangesAsync();
    }
}