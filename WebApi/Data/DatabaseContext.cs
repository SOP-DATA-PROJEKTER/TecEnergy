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

        }


    }
}
