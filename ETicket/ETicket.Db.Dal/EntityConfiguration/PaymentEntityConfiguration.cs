
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Enums;
using ETicket.Db.Domain.Entities;

namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class PaymentEntityConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder
                .Property(e => e.PaymentStatusId)
                .HasConversion<int>()
                .HasDefaultValue(PaymentStatusOption.New)
                .IsRequired();

            builder.Property(e => e.Date)
                .HasColumnType("datetime")
                .IsRequired();

            builder
                .ToTable(typeof(Payment).Name)
                .HasKey(e => e.Id);
        }
    }
}
