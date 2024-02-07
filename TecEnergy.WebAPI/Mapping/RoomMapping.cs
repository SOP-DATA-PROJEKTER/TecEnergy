using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;

namespace TecEnergy.WebAPI.Mapping;

public class RoomMapping
{
    public static SimpleDto RoomToSimpleDto(Room room) => new SimpleDto
    {
        Id = room?.Id ?? Guid.Empty,
        Name = room?.RoomName ?? string.Empty
    };
    
    public static EnergyDto RoomToEnergyDto(Room room, double realtime, double accumulated) => new EnergyDto
    {
        Id = room?.Id ?? Guid.Empty,
        Name = room?.RoomName ?? string.Empty,
        RealTime = realtime,
        Accumulated = accumulated
    };
}
