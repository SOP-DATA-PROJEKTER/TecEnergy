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
        var result = await _repository.GetByIdWithEnergyMetersAsync(id);

        // Flatten the nested EnergyDatas from all EnergyMeters
        var allEnergyDatas = result.EnergyMeters
            .SelectMany(em => em.EnergyDatas)
            .ToList();

        // Filter EnergyDatas based on startDateTime and endDateTime if provided
        if (startDateTime.HasValue && endDateTime.HasValue)
        {
            allEnergyDatas = allEnergyDatas
                .Where(ed => ed.DateTime >= startDateTime && ed.DateTime <= endDateTime)
                .ToList();
        }

        // Calculate the sum of AccumulatedValue from all EnergyDatas
        var impulseCount = result.EnergyMeters
            .SelectMany(em => em.EnergyDatas)
            .GroupBy(ed => ed.EnergyMeterID)
            .Select(group => group.Max(ed => ed.AccumulatedValue))
            .Sum();

        var accumulated = CalculationHelper.CalculateAccumulatedEnergy(impulseCount, 0.001);

        // Calculate the real-time value
        var hoursInDouble = CalculationHelper.CalculateHoursToDouble(startDateTime, endDateTime);
        var realtime = CalculationHelper.GetKilowattsInHours(allEnergyDatas.Count(), hoursInDouble);

        var energyDto = RoomMapping.RoomToEnergyDto(result, realtime, accumulated);

        return energyDto;
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
