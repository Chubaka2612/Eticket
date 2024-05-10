
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Entities;

namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasOne(e => e.User)
                .WithMany(e => e.OrderItems)
                .HasForeignKey(e => e.UserId);

            builder.HasOne(e => e.Payment)
                .WithMany(e => e.OrderItems)
                .HasForeignKey(e => e.PaymentId);

            builder
                .HasOne(e => e.Event)
                .WithMany(e => e.OrderItems)
                .HasForeignKey(e => e.EventId);

            builder
                .HasOne(e => e.Price)
                .WithMany(e => e.OrderItems)
                .HasForeignKey(e => e.PriceId);

            builder
                .HasOne(e => e.Seat)
                .WithOne(e => e.OrderItem)
                .HasForeignKey<Seat>(e => e.Id);

            builder
                .ToTable(typeof(OrderItem).Name)
                .HasKey(e => e.Id);
        }
    }
}
