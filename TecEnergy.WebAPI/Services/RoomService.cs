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

    public async Task<EnergyDto> GetEnergyDtoAsync(Guid id, DateTime? startDateTime, DateTime? endDateTime)
    {
        var result = await _repository.GetByIdWithEnergyMetersFirstAndLastAsync(id, startDateTime, endDateTime);

        //var result = await _repository.GetByIdWithEnergyMetersAsync(id);

        var energyMeterList = result.EnergyMeters.ToList();

        // Flatten the nested EnergyDatas from all EnergyMeters
        //var allEnergyDatas = result.EnergyMeters
        //    .SelectMany(em => em.EnergyDatas)
        //    .ToList();

        //// Filter EnergyDatas based on startDateTime and endDateTime if provided
        //if (startDateTime.HasValue && endDateTime.HasValue)
        //{
        //allEnergyDatas = allEnergyDatas
        //    .Where(ed => ed.DateTime >= startDateTime && ed.DateTime <= endDateTime)
        //    .ToList();
        //}

        ////Calculate the sum of AccumulatedValue from all EnergyDatas
        //var impulseCount = result.EnergyMeters
        //    .SelectMany(em => em.EnergyDatas)
        //    .GroupBy(ed => ed.EnergyMeterID)
        //    .Select(group => group.Max(ed => ed.AccumulatedValue))
        //    .Sum();

        var impulseCount = 0;
        long acc = 0;
        foreach (var item in result.EnergyMeters)
        {
            impulseCount += item.EnergyDatas.Count();
            var x = item.EnergyDatas.OrderBy(x => x.DateTime).LastOrDefault();

            if(x!= null)
                acc += x.AccumulatedValue;
        }

        var accumulated = CalculationHelper.CalculateAccumulatedEnergy(acc, 0.001);

        // Calculate the real-time value
        var hoursInDouble = CalculationHelper.CalculateHoursToDouble(startDateTime, endDateTime);


        //var realtime = CalculationHelper.GetKilowattsInHours(impulseCount, hoursInDouble);
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
}
