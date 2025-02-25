using Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    internal class ProfilConfiguration : IEntityTypeConfiguration<Profil>
    {
        public void Configure(EntityTypeBuilder<Profil> builder)
        {
            builder.ToTable("Profiles");

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
