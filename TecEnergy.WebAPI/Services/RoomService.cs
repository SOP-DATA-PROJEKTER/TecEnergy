using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Runtime.CompilerServices;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;
using TecEnergy.Database.Repositories.Interfaces;
using TecEnergy.WebAPI.Helpers;
using TecEnergy.WebAPI.Mapping;

namespace TecEnergy.WebAPI.Services;

public class RoomService
{
    private readonly IRoomRepository _repository;
    public RoomService(IRoomRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        return result;
    }

    public async Task<SimpleDto> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        var dto = RoomMapping.RoomToSimpleDto(result);
        return dto;
    }

    public async Task<List<SimpleDto>> GetSimpleListByBuildingIdAsync(Guid buildingId)
    {
        List<SimpleDto> dtoList = new();
        var result = await _repository.GetBuildingByIdAsync(buildingId);
        var rooms = result.Rooms.ToList();
        foreach (var item in rooms)
        {
            var dto = RoomMapping.RoomToSimpleDto(item);
            dtoList.Add(dto);
        }
        return dtoList;
    }

    //returns energyDTO of a room with accumulated and realtime value in the given timespan of datetimes
    public async Task<EnergyDto> GetEnergyDtoAsync(Guid id, DateTime? startDateTime, DateTime? endDateTime)
    {
        // Retrieve room data along with associated energy meters and energy data which datetime in within the timespan of startDateTime and endDateTime
        var result = await _repository.GetByIdWithEnergyMetersFirstAndLastAsync(id, startDateTime, endDateTime);

        //impulse count and accumulated value
        var impulseCount = 0;
        long acc = 0;

        // Loop through each energy meter to calculate impulse count and the latest accumulated value
        foreach (var item in result.EnergyMeters)
        {
            impulseCount += item.EnergyDatas.Count();
            var x = item.EnergyDatas.OrderBy(x => x.DateTime).LastOrDefault();

            if(x!= null)
                acc += x.AccumulatedValue;
        }
        // Calculate the accumulated and real-time value.
        // this is quickfix for realtime for now, but implement method in calculationhelper
        var accumulated = CalculationHelper.CalculateAccumulatedEnergy(acc, 0.001);
        double realtime = impulseCount * 60f / 1000f;

        var energyDto = RoomMapping.RoomToEnergyDto(result, realtime, accumulated);
        return energyDto;
    }

    public async Task<List<EnergyMeter>> GetEnergyMeterListDtoByRoomId(Guid roomId, DateTime? startDateTime, DateTime? endDateTime)
    {
        var result = await _repository.GetByIdWithEnergyMetersFirstAndLastAsync(roomId, startDateTime, endDateTime);
        return result.EnergyMeters;
    }

    public async Task AddAsync(Room room)
    {
        await _repository.AddAsync(room);
    }

    public async Task UpdateAsync(Guid id, Room room)
    {
        await _repository.UpdateAsync(room);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Room>> SearchAsync(string searchInput)
    {
        var result = await _repository.SearchAsync(searchInput);
        return result;
    }

    public async Task<Room> GetFirstRoomAsync()
    {
        var result = await _repository.GetFirstRoomAsync();
        return result;
    }


    public async Task<ICollection<DailyAccumulated>> GetAccumulatedEnergyForARoom(Guid roomId, DateTime startTime, DateTime endTime)
    {
        var result = await _repository.GetDailyAccumulationAsync(roomId, startTime, endTime);
        return result;
    }
}
