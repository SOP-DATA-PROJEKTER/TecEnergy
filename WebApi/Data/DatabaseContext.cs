using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            
        }

        public DbSet<DailyAccumulated> DailyAccumulations { get; set; }
        public DbSet<EnergyData> EnergyData { get; set; }
        public DbSet<EnergyMeter> EnergyMeters { get; set; }
        public DbSet<Room> Rooms { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnergyMeter>()
                .HasOne(e => e.Room)
                .WithMany(r => r.EnergyMeters)
                .HasForeignKey(e => e.RoomId);

            modelBuilder.Entity<EnergyData>()
                .HasOne(e => e.EnergyMeter)
                .WithMany(m => m.EnergyDatas)
                .HasForeignKey(e => e.EnergyMeterId);

        }


    }
}
