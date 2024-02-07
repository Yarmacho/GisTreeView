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

            builder.Property(s => s.Id).HasColumnName("ID");
            builder.Property(s => s.GasId).HasColumnName("GasId");
            builder.Property(s => s.Name).HasColumnName("Name");

            builder.HasKey(s => s.Id);
        }
    }
}
