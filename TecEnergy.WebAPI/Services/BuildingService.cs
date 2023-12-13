using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Models.DtoModels;
using TecEnergy.Database.Repositories.Interfaces;
using TecEnergy.WebAPI.Helpers;
using TecEnergy.WebAPI.Mapping;

namespace TecEnergy.WebAPI.Services;

public class BuildingService
{
    private readonly IBuildingRepository _repository;
    public BuildingService(IBuildingRepository repository)
    {
            _repository = repository;
    }

    public async Task<IEnumerable<Building>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        return result;
    }
    public async Task<SimpleDto> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        var dto = BuildingMapping.BuildingToSimpleDto(result);
        return dto;
    }
    public async Task<EnergyDto> GetBuildingEnergyDtoAsync(Guid buildingId, DateTime? startDateTime, DateTime? endDateTime)
    {
        var building = await _repository.GetByIdWithRoomsAndMetersAsync(buildingId);

        // Flatten the nested EnergyDatas from all EnergyMeters in all Rooms
        var allEnergyDatas = building.Rooms
            .SelectMany(room => room.EnergyMeters.SelectMany(em => em.EnergyDatas))
            .ToList();

        // Filter EnergyDatas based on startDateTime and endDateTime if provided
        if (startDateTime.HasValue && endDateTime.HasValue)
        {
            allEnergyDatas = allEnergyDatas
                .Where(ed => ed.DateTime >= startDateTime && ed.DateTime <= endDateTime)
                .ToList();
        }

        // Calculate the sum of AccumulatedValue from all EnergyDatas
        var impulseCount = allEnergyDatas
            .GroupBy(ed => ed.EnergyMeterID)
            .Select(group => group.Max(ed => ed.AccumulatedValue))
            .Sum();

        var accumulated = CalculationHelper.CalculateAccumulatedEnergy(impulseCount, 0.001);

        // Calculate the real-time value
        var hoursInDouble = CalculationHelper.CalculateHoursToDouble(startDateTime, endDateTime);
        var realtime = CalculationHelper.GetKilowattsInHours(allEnergyDatas.Count(), hoursInDouble);

        var energyDto = BuildingMapping.BuildingToEnergyDto(building, realtime, accumulated);

        return energyDto;
    }


    public async Task AddAsync(Building buidling)
    {
        await _repository.AddAsync(buidling);
    }

    public async Task UpdateAsync(Guid id, Building buidling)
    {
        await _repository.UpdateAsync(buidling);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Building>> SearchAsync(string searchInput)
    {
        var result = await _repository.SearchAsync(searchInput);
        return result;
    }
}
