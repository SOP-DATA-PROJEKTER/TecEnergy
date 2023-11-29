using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;

namespace TecEnergy.WebAPI.Mapping;

public class EnergyMeterMappings
{
    public static SimpleDto EnergyMeterToSimpleDto(EnergyMeter energyMeter) => new SimpleDto
    {
        Id = energyMeter?.Id ?? Guid.Empty,
        Name = energyMeter?.MeasurementPointName ?? string.Empty
    };
}
