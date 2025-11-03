using CommunityFunctions.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace CommunityFunctions.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<EventOrganiser> EventOrganisers { get; set; }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Event>().Property(e => e.Category).HasConversion<string>();
            model.Entity<EventOrganiser>().HasKey(eo => new { eo.EventId, eo.OrganiserId });

            model.Entity<EventOrganiser>()
                .HasOne(eo => eo.Event)
                .WithMany(e => e.EventOrganisers)
                .HasForeignKey(eo => eo.EventId);

            model.Entity<EventOrganiser>()
                .HasOne(eo => eo.Organiser)
                .WithMany(o => o.EventOrganisers)
                .HasForeignKey(eo => eo.OrganiserId);

            model.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId);
             
            
            model.Entity<Customer>()
            .HasKey(u => u.Email);

            model.Entity<Customer>()
                .Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            base.OnModelCreating(model);
        }
    }
}
