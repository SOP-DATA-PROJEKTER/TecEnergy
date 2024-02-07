using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecEnergy.Database.Models.DataModels;

namespace TecEnergy.Database.Models.DtoModels
{
    public class EnergyDataDto
    {
        public Guid EnergyMeterID { get; set; }
        public long AccumulatedValue { get; set; }
    }
}
