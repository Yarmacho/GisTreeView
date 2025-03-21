﻿using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    internal class GasConfiguration : IEntityTypeConfiguration<Gas>
    {
        public void Configure(EntityTypeBuilder<Gas> builder)
        {
            builder.ToTable("Gas");

            builder.Property(r => r.Id).HasColumnName("ID")
                .ValueGeneratedOnAdd();
            builder.Property(r => r.Name).HasColumnName("NAME");
            builder.Property(r => r.ExperimentId).HasColumnName("EXPERIMENTID");
            builder.Property(r => r.X);
            builder.Property(r => r.Y);
            builder.Ignore(r => r.Shape);

            builder.HasKey(r => r.Id);
        }
    }
}
