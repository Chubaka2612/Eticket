
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Entities;

namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder
                  .HasOne(e => e.Order)
                  .WithMany(e => e.OrderItems)
                  .HasForeignKey(e => e.OrderId);

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
