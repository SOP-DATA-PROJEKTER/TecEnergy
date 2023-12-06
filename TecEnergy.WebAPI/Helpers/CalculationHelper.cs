using Microsoft.AspNetCore.Routing.Constraints;

namespace TecEnergy.WebAPI.Helpers;

public class CalculationHelper
{
    public static double CalculateHoursToDouble(DateTime startTime, DateTime endTime)
    {
        TimeSpan duration = endTime - startTime;
        var hours = duration.TotalHours;
        return hours;
    }

    public static double GetRealTimeKilowattsInHours(int watts, double hours)
    {
        //var hours = CalculateHoursToDouble(startTime, endTime);

        var kWh = (watts * hours) / 1000;
        return kWh;
    }

    //public static double GetAccumulatedKilowattsInHours(Func<DateTime, double> powerFunction, DateTime startTime, DateTime endTime, double timeStep = 0.001)
    //{
    //    double accumulatedEnergy = 0;
    //    for (DateTime time = startTime; time < endTime; time = time.AddSeconds(timeStep))
    //    {
    //        double power = powerFunction(time);
    //        accumulatedEnergy += power * timeStep;
    //    }
    //    return accumulatedEnergy;
    //}
}
