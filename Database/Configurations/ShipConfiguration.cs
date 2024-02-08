using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    internal class ShipConfiguration : IEntityTypeConfiguration<Ship>
    {
        public void Configure(EntityTypeBuilder<Ship> builder)
        {
            builder.ToTable("Ships");

            builder.Property(s => s.Id).HasColumnName("ID")
                .ValueGeneratedOnAdd();
            builder.Property(s => s.Name).HasColumnName("NAME");
            builder.Property(s => s.SceneId).HasColumnName("SCENEID");

            builder.HasKey(s => s.Id);
        }
    }
}
