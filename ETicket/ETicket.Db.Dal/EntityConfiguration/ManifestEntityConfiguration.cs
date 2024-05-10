
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Entities;

namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class ManifestEntityConfiguration : IEntityTypeConfiguration<Manifest>
    {
        public void Configure(EntityTypeBuilder<Manifest> builder)
        {
            builder
                .HasOne(e => e.Venue)
                .WithOne(e => e.Manifest)
                .HasForeignKey<Venue>(e => e.Id);

            builder
                .ToTable(typeof(Manifest).Name)
                .HasKey(e => e.Id);
        }
    }
}
