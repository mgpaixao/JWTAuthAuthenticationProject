using JWTAuthAuthentication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWTAuthAuthentication.ContextMapping
{
    public class RolesMap : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Role");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(x => x.Name)
                .HasMaxLength(128)
                .HasColumnType("VARCHAR")
                .HasColumnName("Name");

        }
    }
}
