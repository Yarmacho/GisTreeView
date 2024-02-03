using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    internal class ExperimentConfiguration : IEntityTypeConfiguration<Experiment>
    {
        public void Configure(EntityTypeBuilder<Experiment> builder)
        {
            builder.ToTable("Experiments");

            builder.Property(e => e.Id).HasColumnName("ID")
                .ValueGeneratedOnAdd();
            builder.Property(e => e.Name).HasColumnName("NAME");
            builder.Property(e => e.Description).HasColumnName("DESC");
            builder.Property(e => e.GasId).HasColumnName("GASID");

            builder.HasKey(e => e.Id);
        }
    }
}
