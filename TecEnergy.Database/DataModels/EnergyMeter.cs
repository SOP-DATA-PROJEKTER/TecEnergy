using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecEnergy.Database.DataModels;
public class EnergyMeter
{
    public Guid Id { get; set; }
    public Guid RoomID { get; set; }
    public string ConnectionState { get; set; } //connected, Not connected, error 
    public string ReadingFrequency { get; set; } //how oftens it reads impulses (realtime or accumulated)
    public string MeasurementPointName { get; set; } //f.x. GPIO4 or position in energycabin
    public string MeasurementType { get; set; } //real-time or accumulated
    public DateTime InstallmentDate { get; set; } //Date the monitor got installed
    public string? MeasurementPointComment { get; set; } //where in the room it's located, what type of room/education...
    public Room? Room { get; set; }
}
