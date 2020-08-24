using FWTL.Aggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FWTL.Auth.Database.Configuration
{
    public class TelegramAccountConfiguration : IEntityTypeConfiguration<TelegramAccount>
    {
        public void Configure(EntityTypeBuilder<TelegramAccount> builder)
        {
            builder.HasKey(vf => new { vf.UserId, vf.Number });
            builder.Property(x => x.Number).IsRequired().HasMaxLength(50);
        }
    }
}