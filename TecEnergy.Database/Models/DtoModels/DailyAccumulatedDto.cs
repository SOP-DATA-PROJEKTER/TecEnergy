using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecEnergy.Database.Models.DataModels;

namespace TecEnergy.Database.Models.DtoModels
{
    public class DailyAccumulatedDto
    {
        public DateTime DateTime { get; set; }
        public long DailyAccumulatedValue { get; set; }
    }
}
