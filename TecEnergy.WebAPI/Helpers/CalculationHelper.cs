namespace TecEnergy.WebAPI.Helpers;

public class CalculationHelper
{
    public static double CalculateHoursToDouble(DateTime startTime, DateTime endTime)
    {
        TimeSpan duration = endTime - startTime;
        var hours = duration.TotalHours;
        return hours;
    }

    public static double GetRealTimeKilowattsInHours(int watts, DateTime startTime, DateTime endTime)
    {
        var hours = CalculateHoursToDouble(startTime, endTime);

        var kWh = (watts * hours) / 1000;
        return kWh;
    }
}
