using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            
        }

        public DbSet<Building> Buildings { get; set; }
        public DbSet<DailyAccumulated> DailyAccumulations { get; set; }
        public DbSet<EnergyData> EnergyData { get; set; }
        public DbSet<EnergyMeter> EnergyMeters { get; set; }
        public DbSet<Room> Rooms { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // building and room
            modelBuilder.Entity<Building>()
                .HasMany(b => b.Rooms)
                .WithOne(r => r.Building)
                .HasForeignKey(r => r.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);


            // room and energy-meter
            modelBuilder.Entity<Room>()
                .HasMany(r => r.EnergyMeters)
                .WithOne(em => em.Room)
                .HasForeignKey(em => em.RoomId)
                .OnDelete(DeleteBehavior.Cascade);


        }


    }
}
