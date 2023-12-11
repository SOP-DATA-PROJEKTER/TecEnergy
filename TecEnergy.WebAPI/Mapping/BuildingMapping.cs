using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;

namespace TecEnergy.WebAPI.Mapping;

public class BuildingMapping
{
    public static SimpleDto BuildingToSimpleDto(Building building) => new SimpleDto
    {
        Id = building?.Id ?? Guid.Empty,
        Name = building?.BuildingName ?? string.Empty
    };
    public static EnergyDto BuildingToEnergyDto(Building building, double realtime, double accumulated) => new EnergyDto
    {
        Id = building?.Id ?? Guid.Empty,
        Name = building?.BuildingName ?? string.Empty,
        RealTime = realtime,
        Accumulated = accumulated
    };
}
