
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Enums;
using ETicket.Db.Domain.Entities;

namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class PaymentStatusEntityConfiguration : IEntityTypeConfiguration<PaymentStatus>
    {
        public void Configure(EntityTypeBuilder<PaymentStatus> builder)
        {
            builder
                .Property(e => e.PaymentStatusId)
                .HasConversion<int>();

            builder.HasData(
                Enum.GetValues(typeof(PaymentStatusOption))
                .Cast<PaymentStatusOption>()
                .Select(e => new PaymentStatus()
                {
                    PaymentStatusId = e,
                    Name = e.ToString()
                })
            );

            builder
                .ToTable(typeof(PaymentStatus).Name);
        }
    }
}
