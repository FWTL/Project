using FWTL.Aggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FWTL.Auth.Database.Configuration
{
    public class TelegramAccountConfiguration : IEntityTypeConfiguration<TelegramAccount>
    {
        public void Configure(EntityTypeBuilder<TelegramAccount> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(b => b.AccountId).IsRequired().HasMaxLength(50);
        }
    }
}