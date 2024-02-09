using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    internal class RoutePointConfiguration : IEntityTypeConfiguration<RoutePoint>
    {
        public void Configure(EntityTypeBuilder<RoutePoint> builder)
        {
            builder.ToTable("RoutePoints");

            builder.Property(p => p.X);
            builder.Property(p => p.Y);
            builder.Property(p => p.RouteId);
            builder.Property(p => p.Id);

            builder.HasKey(p => new { p.RouteId, p.Id });
        }
    }
}
