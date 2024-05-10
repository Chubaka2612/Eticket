
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Entities;

namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(e => e.User)
                .WithMany(e => e.Orders)
                .HasForeignKey(e => e.UserId);

            builder.HasOne(e => e.Payment)
                .WithOne(e => e.Order)
                .HasForeignKey<Payment>(e => e.Id);

            builder.Property(e => e.Date)
                .HasColumnType("datetime")
                .IsRequired();

            builder
              .HasOne(e => e.Event)
              .WithMany(e => e.Orders)
              .HasForeignKey(e => e.EventId);

            builder
                .ToTable(typeof(Order).Name)
                .HasKey(e => e.Id);
        }
    }
}
