
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Entities;

namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class RowEntityConfiguration : IEntityTypeConfiguration<Row>
    {
        public void Configure(EntityTypeBuilder<Row> builder)
        {
            builder.Property(p => p.Number)
                .IsRequired();

            builder
                .HasOne(e => e.Section)
                .WithMany(e => e.Rows)
                .HasForeignKey(e => e.SectionId);

            builder
                .ToTable(typeof(Row).Name)
                .HasKey(e => e.Id);
        }
    }
}
