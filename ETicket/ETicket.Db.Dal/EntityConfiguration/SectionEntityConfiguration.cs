
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Entities;

namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class SectionEntityConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .HasOne(e => e.Manifest)
                .WithMany(e => e.Sections)
                .HasForeignKey(e => e.ManifestId);

            builder
                .ToTable(typeof(Section).Name)
                .HasKey(e => e.Id);
        }
    }
}
