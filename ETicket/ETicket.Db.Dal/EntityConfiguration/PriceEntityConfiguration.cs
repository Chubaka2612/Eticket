
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Entities;


namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class PriceEntityConfiguration : IEntityTypeConfiguration<Price>
    {
        public void Configure(EntityTypeBuilder<Price> builder)
        {
            builder.Property(e => e.Name)
               .HasMaxLength(100)
               .IsRequired();

            builder.Property(e => e.Amount)
                .HasPrecision(precision: 14, scale: 6);

            builder
            .ToTable(typeof(Price).Name)
            .HasKey(e => e.Id);
        }
    }
}
