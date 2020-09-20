using FWTL.Aggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FWTL.Auth.Database.Configuration
{
    public class TelegramAccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(b => b.ExternalId).IsRequired().HasMaxLength(50);
        }
    }
}