using Microsoft.AspNetCore.Routing.Constraints;

namespace TecEnergy.WebAPI.Helpers;

public class CalculationHelper
{
    public static double CalculateHoursToDouble(DateTime? startTime, DateTime? endTime)
    {
        TimeSpan? duration = endTime - startTime;
        var hours = duration.Value.TotalHours;
        return hours;
    }

    public static double GetKilowattsInHours(int watts, double hours)
    {
        var kWh = (watts * hours) / 1000;
        return kWh;
    }

    public static double CalculateAccumulatedEnergy(long impulseCount, double conversionFactorKWhPerImpulse)
    {
        return impulseCount * conversionFactorKWhPerImpulse;
    }
}
