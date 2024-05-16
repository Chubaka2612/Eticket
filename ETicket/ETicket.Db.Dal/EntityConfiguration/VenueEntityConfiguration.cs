
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Entities;


namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class VenueEntityConfiguration : IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> builder)
        {
            builder.Property(e => e.Name)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(e => e.City)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.Country)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.Address)
                   .HasMaxLength(255)
                   .IsRequired();

            builder
                .HasMany(e => e.EventVenues)
                .WithOne(e => e.Venue)
                .HasForeignKey(e => e.VenueId);

            builder
                .ToTable(typeof(Venue).Name)
                .HasKey(e => e.Id);
        }
    }
}
