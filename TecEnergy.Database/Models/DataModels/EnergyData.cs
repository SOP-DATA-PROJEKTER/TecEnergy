using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecEnergy.Database.Models.DataModels;
public class EnergyData
{
    public Guid Id { get; set; }
    public Guid EnergyMeterID { get; set; }
    public DateTime DateTime { get; set; }
    public long AccumulatedValue { get; set; }
    public EnergyMeter? EnergyMeter { get; set; }
}
