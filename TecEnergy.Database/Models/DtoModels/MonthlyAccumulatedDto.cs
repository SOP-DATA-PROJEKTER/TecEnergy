using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecEnergy.Database.Models.DtoModels
{
    public class MonthlyAccumulatedDto
    {
        public long MonthlyAccumulatedValue { get; set; }
        public DateOnly Month { get; set; }
    }
}
