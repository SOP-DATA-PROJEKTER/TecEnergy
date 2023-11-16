using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecEnergy.Database.DataModels;
public class EnergyData
{
    public Guid Id { get; set; }
    public Guid EnergyMeterID { get; set; }
    //public string MeasurementUnit { get; set; } //fx J, kW, kWh, Lm/W
    public DateTime DateTime { get; set; }
    public EnergyMeter? EnergyMeter { get; set; }
}
