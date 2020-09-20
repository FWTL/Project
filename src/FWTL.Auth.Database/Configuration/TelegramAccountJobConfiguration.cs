using FWTL.Aggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FWTL.Auth.Database.Configuration
{
    public class TelegramAccountJobConfiguration : IEntityTypeConfiguration<TelegramAccountJob>
    {
        public void Configure(EntityTypeBuilder<TelegramAccountJob> builder)
        {
            builder.HasKey(x => new { x.JobId, x.TelegramAccountId });
        }
    }
}