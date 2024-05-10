
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Enums;
using ETicket.Db.Domain.Entities;

namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class SeatEntityConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.Property(p => p.Number)
                .IsRequired();
            
            builder
                .Property(e => e.SeatStatusId)
                .HasConversion<int>()
                .HasDefaultValue(SeatStatusOption.Available)
                .IsRequired();

            builder
                .HasOne(e => e.Row)
                .WithMany(e => e.Seats)
                .HasForeignKey(e => e.RowId);

            builder
                .ToTable(typeof(Seat).Name)
                .HasKey(e => e.Id);
        }
    }
}
