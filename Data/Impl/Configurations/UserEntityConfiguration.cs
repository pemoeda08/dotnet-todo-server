using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoServer.Data.Entities;

namespace TodoServer.Data.Impl.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users")
                .HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.HashedPassword)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}