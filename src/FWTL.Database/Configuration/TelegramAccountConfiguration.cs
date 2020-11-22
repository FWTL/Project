using FWTL.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FWTL.Database.Configuration
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(b => b.ExternalAccountId).IsRequired().HasMaxLength(50);
        }
    }
}