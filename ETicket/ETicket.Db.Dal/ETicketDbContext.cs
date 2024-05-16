

using ETicket.Db.Dal.EntityConfiguration;
using ETicket.Db.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETicket.Db.Dal
{
    public class ETicketDbContext : DbContext
    {

        public ETicketDbContext(DbContextOptions options) : base(options)
        {

        }

        public virtual DbSet<Event> Events { get; set; }

        public virtual DbSet<Manifest> Manifests { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<Price> Prices { get; set; }

        public virtual DbSet<Row> Rows { get; set; }

        public virtual DbSet<Seat> Seats { get; set; }

        public virtual DbSet<Section> Sections { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Venue> Venues { get; set; }

        public virtual DbSet<SeatStatus> SeatStatuses { get; set; }

        public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ManifestEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentStatusEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PriceEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RowEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SeatEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SeatStatusEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SectionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new VenueEntityConfiguration());
            
            modelBuilder
                .Entity<Venue>()
                .HasMany(e => e.Events)
                .WithMany(e => e.Venues)
                .UsingEntity<EventVenue>(e => e
                .HasOne(ee => ee.Event)
                .WithMany(ev => ev.EventVenues)
                .HasForeignKey(ev => ev.EventId), e => e
                .HasOne(v => v.Venue)
                .WithMany(ev => ev.EventVenues)
                .HasForeignKey(ev => ev.VenueId), e =>
                {
                    e.HasKey(t => new { t.VenueId, t.EventId });
                    e.ToTable(nameof(EventVenue));
                });

            modelBuilder.Entity<Venue>()
                .HasOne(s => s.Manifest)
                .WithOne(ad => ad.Venue)
                .HasForeignKey<Manifest>(ad => ad.VenueId);

            modelBuilder.Entity<Seat>()
               .HasOne(s => s.OrderItem)
               .WithOne(ad => ad.Seat)
               .HasForeignKey<OrderItem>(ad => ad.SeatId);

        }
    }
}
