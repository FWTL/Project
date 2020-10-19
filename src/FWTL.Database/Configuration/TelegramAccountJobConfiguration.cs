using FWTL.Aggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FWTL.Auth.Database.Configuration
{
    public class TelegramAccountJobConfiguration : IEntityTypeConfiguration<AccountJob>
    {
        public void Configure(EntityTypeBuilder<AccountJob> builder)
        {
            builder.HasKey(x => new { x.JobId, x.AccountId });
        }
    }
}