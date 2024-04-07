using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Configurations
{
    internal class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.ToTable("Routes");

            builder.Property(r => r.Id).HasColumnName("ID")
                .ValueGeneratedOnAdd();
            builder.Property(r => r.Name).HasColumnName("NAME");
            builder.Property(r => r.ShipId).HasColumnName("SHIPID");
            builder.Property(r => r.Description).HasColumnName("DESCR");
            builder.Ignore(r => r.Shape);

            builder.HasKey(r => r.Id);

            builder.HasMany(r => r.Points)
                .WithOne()
                .HasForeignKey(p => p.RouteId)
                .HasPrincipalKey(r => r.Id);
        }
    }
}
