using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoServer.Data.Entities;

namespace TodoServer.Data.Impl.Configurations {

    public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
    {
        public void Configure(EntityTypeBuilder<TodoItem> builder)
        {
            builder.ToTable("todos")
                .HasKey(x => x.Id);
            

            builder.Property(x => x.Id)
                .HasIdentityOptions(1, 1);

            builder.Property(x => x.Text)
                .IsRequired()
                .HasColumnType("text");
            
            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()")
                .IsRequired();
            
            builder.HasOne(t => t.CreatedBy)
                .WithMany(u => u.Todos);
        }
    }
}