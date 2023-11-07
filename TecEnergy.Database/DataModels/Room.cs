using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecEnergy.Database.DataModels;
public class Room
{
    public Guid Id { get; set; }
    public Guid? BuildingID { get; set; }
    public string? RoomName { get; set; }// E213
    public string? RoomComment { get; set; } //specify rooms purpose, department etc..
    public Building? Building { get; set; }
    public List<EnergyMeter>? EnergyMeters { get; set; }
}
