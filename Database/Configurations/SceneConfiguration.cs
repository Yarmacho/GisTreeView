using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    internal class SceneConfiguration : IEntityTypeConfiguration<Scene>
    {
        public void Configure(EntityTypeBuilder<Scene> builder)
        {
            builder.ToTable("Scenes");

            builder.Property(s => s.Id).HasColumnName("ID")
                .ValueGeneratedOnAdd();
            builder.Property(s => s.GasId).HasColumnName("GasId");
            builder.Property(s => s.Name).HasColumnName("Name");
            builder.Property(s => s.Side);
            builder.Property(s => s.Area);
            builder.Property(s => s.Angle);
            builder.Ignore(r => r.Shape);

            builder.HasKey(s => s.Id);
        }
    }
}
