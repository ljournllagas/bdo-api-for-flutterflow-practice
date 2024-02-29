using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence.FluentEntityConfigs
{
    public class AuthConfig : IEntityTypeConfiguration<Auth>
    {
        public void Configure(EntityTypeBuilder<Auth> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(a => a.EmailAddress).HasMaxLength(30).IsRequired();

            builder.Property(a => a.Password).HasMaxLength(20).IsRequired();
        }
    }
}
