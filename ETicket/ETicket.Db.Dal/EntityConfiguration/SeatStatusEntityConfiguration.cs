
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Enums;
using ETicket.Db.Domain.Entities;

namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class SeatStatusEntityConfiguration : IEntityTypeConfiguration<SeatStatus>
    {
        public void Configure(EntityTypeBuilder<SeatStatus> builder)
        {
            builder
                .Property(e => e.SeatStatusId)
                .HasConversion<int>();
            
            builder.HasData(
                Enum.GetValues(typeof(SeatStatusOption))
                .Cast<SeatStatusOption>()
                .Select(e => new SeatStatus()
                {
                    SeatStatusId = e,
                    Name = e.ToString()
                })
            );

            builder
                .ToTable(typeof(SeatStatus).Name);
        }
    }
}
