using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecEnergy.Database.DataModels;
public class Building
{
    public Guid Id { get; set; }
    public string? BuildingName { get; set; } // Ballerup, Hvidovre, frederiksberg
    public string? Address { get; set; }
    public List<Room>? Rooms { get; set; } 
}
