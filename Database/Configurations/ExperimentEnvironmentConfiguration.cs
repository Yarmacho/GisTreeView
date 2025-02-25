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
    internal class ExperimentEnvironmentConfiguration : IEntityTypeConfiguration<ExperimentEnvironment>
    {
        public void Configure(EntityTypeBuilder<ExperimentEnvironment> builder)
        {
            builder.ToTable("Environment");
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.ExperimentId).IsUnique();
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }
}
