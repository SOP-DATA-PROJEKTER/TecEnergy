using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;

namespace TecEnergy.WebAPI.Mapping
{
    public class DailyAccumulatedMappings
    {
        public static class DailyAccumulatedToDailyAccumulatedDto
        {
            public static DailyAccumulatedDto Map(DailyAccumulated dailyAccumulated) => new DailyAccumulatedDto
            {
                DateTime = dailyAccumulated.DateTime,
                DailyAccumulatedValue = dailyAccumulated.DailyAccumulatedValue
            };



        }
    }
}
