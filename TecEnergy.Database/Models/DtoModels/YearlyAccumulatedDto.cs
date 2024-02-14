using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecEnergy.Database.Models.DtoModels
{
    public class YearlyAccumulatedDto
    {
        public DateOnly Date { get; set; }
        public long Accumulated { get; set; }
    }
}
