
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ETicket.Db.Domain.Entities;


namespace ETicket.Db.Dal.EntityConfiguration
{
    internal class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(e => e.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(e => e.Phone)
                .HasMaxLength(15)
                .IsRequired();

            builder
                .HasIndex(e => e.Phone)
                .IsUnique();

            builder
                .HasIndex(e => e.Email)
                .IsUnique();

            builder
                .Property(e => e.Email)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
