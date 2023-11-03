using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TecEnergy.Database.DataModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TecEnergy.Database;
public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    //public DbSet<Building> Buildings { get; set; }
    //public DbSet<Room> Rooms { get; set; }
    public DbSet<EnergyMeter> EnergyMeters { get; set; }
    public DbSet<EnergyData> EnergyData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Room>()
        //.HasOne<Building>()
        //.WithMany(x => x.)
        //.HasForeignKey(em => em.RoomID);

        modelBuilder.Entity<EnergyData>()
            .Property(e => e.Value)
            .HasColumnType("decimal(18, 2)"); // 18 is the precision, and 2 is the scale
    }
}
