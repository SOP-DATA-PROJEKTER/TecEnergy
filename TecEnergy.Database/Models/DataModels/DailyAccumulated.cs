using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecEnergy.Database.Models.DataModels
{

    [Index(nameof(DateTime), IsUnique = true)] // makes an index from the DateTime so it is easier to query
    public class DailyAccumulated
    {
        public Guid Id { get; set; }
        public Room Room { get; set; }
        public Guid RoomId { get; set; }
        public DateTime DateTime { get; set; }
        public long DailyAccumulatedValue { get; set; }
    }
}
