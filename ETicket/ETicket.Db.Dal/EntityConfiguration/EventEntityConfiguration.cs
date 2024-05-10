
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Entities;

namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class EventEntityConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.Property(e => e.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Date)
                .HasColumnType("datetime")
                .IsRequired();

            builder
                .HasMany(e => e.EventVenues)
                .WithOne(e => e.Event)
                .HasForeignKey(e => e.EventId);

            builder
                .ToTable(typeof(Event).Name)
                .HasKey(e => e.Id);
        }
    }
}
